using UnityEngine;
using Thirdweb;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine.Networking;
using Org.BouncyCastle.Bcpg;
using System.Numerics;
using Michsky.UI.Reach;

[System.Serializable]
public class ChainData
{
    public string identifier;
    public string chainId;
    public string rpcOverride;

    public ChainData(string identifier, string chainId, string rpcOverride)
    {
        this.identifier = identifier;
        this.chainId = chainId;
        this.rpcOverride = rpcOverride;
    }
}

public class WalletDisconnectedEvent : UnityEvent<bool> { }
public class WalletConnectEvent : UnityEvent<bool> { }
public class NetworkChangeEvent : UnityEvent<int> { }
public class NFTsLoadedEvent : UnityEvent<List<NFT>> { }

public class ThirdwebManager : MonoBehaviour
{
    [Header("REQUIRED SETTINGS")]
    [Tooltip("The chain to initialize the SDK with")]
    public string chain;

    [Header("CHAIN DATA")]
    [Tooltip("Support any chain by adding it to this list from the inspector")]
    public List<ChainData> supportedChainData = new List<ChainData>()
    {
        new ChainData("ethereum", "1", null),
        new ChainData("goerli", "5", null),
        new ChainData("polygon", "137", null),
        new ChainData("mumbai", "80001", null),
        new ChainData("fantom", "250", null),
        new ChainData("fantom-testnet", "4002", null),
        new ChainData("avalanche", "43114", null),
        new ChainData("avalanche-fuji", "43113", null),
        new ChainData("optimism", "10", null),
        new ChainData("optimism-goerli", "420", null),
        new ChainData("arbitrum", "42161", null),
        new ChainData("arbitrum-goerli", "421613", null),
        new ChainData("binance", "56", null),
        new ChainData("binance-testnet", "97", null),
    };

    [Header("STORAGE OPTIONS")]
    [Tooltip("IPFS Gateway Override")]
    public string storageIpfsGatewayUrl = "https://gateway.ipfscdn.io/ipfs/";

    [Header("OZ DEFENDER OPTIONS")]
    [Tooltip("Gasless Transaction Support")]
    public string relayerUrl = null;
    public string relayerForwarderAddress = null;

    public ThirdwebSDK SDK;

    public static ThirdwebManager Instance;

    public string PK_NFT_CONTRACT_ADDRESS; 
    public string PK_RACE_CONTRACT_ADDRESS; 
    public string PK_TOKEN_ADDRESS; 
    public Contract pkNftContract;
    public Contract pkRaceContract;
    public Contract pkTokenContract;
    public bool isLoadingNFTBalance = false;
    public bool isLoadingNFTData = false;
    public List<NFT> walletNFTs = new List<NFT>();
    [SerializeField] public UIPopup uiPopup;

    public WalletDisconnectedEvent walletDisconnectedEvent = new WalletDisconnectedEvent();
    public WalletConnectEvent walletConnectedEvent = new WalletConnectEvent();
    public NetworkChangeEvent walletNetworkChangeEvent = new NetworkChangeEvent();
    public NFTsLoadedEvent nftsLoadedEvent = new NFTsLoadedEvent();

    private List<NFT> LOCAL_NFT_LIST = new List<NFT>()
            {
                new NFT()
                {
                    metadata = new NFTMetadata()
                    {
                        id = "1",
                        uri = "ipfs://QmNcAQLHkWoCjh9kaoitJH9XA7jEFpWbUr5TuYTHSFsPkU/1",
                        description = "Indulge in luxury driving with the Phantom. Its elegant curves and superior performance make it perfect for those who demand the best.",
                        image = "https://gateway.ipfscdn.io/ipfs/QmR9Gc5dSC1pAnx8ckiTqifo1MEuweJLq4iwtUirp8m9Fw/1.png",
                        name = "Phantom",
                        external_url = "https://www.partykarts.io/phantom",
                    },
                    owner = "0x137d0D33ebaD3036241dF6031FA553513a7E18eF",
                    type = "ERC721",
                    supply = 1,
                    quantityOwned = 0
                }
            };

    private void Awake()
    {
        // Single persistent instance at all times.

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Debug.LogWarning("Two ThirdwebManager instances were found, removing this one.");
            Destroy(this.gameObject);
            return;
        }

        // Inspector chain data dictionary.
        ChainData currentChain = GetChainData(chain);

        // Chain ID must be provided on native platforms.

        int chainId = -1;

        if (!Utils.IsWebGLBuild())
        {
            if (string.IsNullOrEmpty(currentChain.chainId))
                throw new UnityException("You must provide a Chain ID on native platforms!");

            if (!int.TryParse(currentChain.chainId, out chainId))
                throw new UnityException("The Chain ID must be a non-negative integer!");
        }

        // Must provide a proper chain identifier (https://thirdweb.com/dashboard/rpc) or RPC override.

        string chainOrRPC = null;

        if (!string.IsNullOrEmpty(currentChain.rpcOverride))
        {
            if (!currentChain.rpcOverride.StartsWith("https://") && currentChain.identifier != "ganache")
                throw new UnityException("RPC overrides must start with https:// !");
            else
                chainOrRPC = currentChain.rpcOverride;
        }
        else
        {
            if (string.IsNullOrEmpty(currentChain.identifier))
                throw new UnityException("When not providing an RPC, you must provide a chain identifier!");
            else
                chainOrRPC = currentChain.identifier;
        }

        // Set up storage and gasless options (if an)

        var options = new ThirdwebSDK.Options()
        {
            wallet = new ThirdwebSDK.WalletOptions()
            {
                appName = "PartyKarts.io",
            }
        };

        if (!string.IsNullOrEmpty(storageIpfsGatewayUrl))
        {
            options.storage = new ThirdwebSDK.StorageOptions() { ipfsGatewayUrl = storageIpfsGatewayUrl };
        }
        if (!string.IsNullOrEmpty(relayerUrl) && !string.IsNullOrEmpty(relayerForwarderAddress))
        {
            options.gasless = new ThirdwebSDK.GaslessOptions()
            {
                openzeppelin = new ThirdwebSDK.OZDefenderOptions() { relayerUrl = this.relayerUrl, relayerForwarderAddress = this.relayerForwarderAddress, }
            };
        }

        SDK = new ThirdwebSDK(chainOrRPC, chainId, options);
        pkNftContract = SDK.GetContract(PK_NFT_CONTRACT_ADDRESS);
        pkRaceContract = SDK.GetContract(PK_RACE_CONTRACT_ADDRESS); //, racingGameABI);
        pkTokenContract = SDK.GetContract(PK_TOKEN_ADDRESS);

        Debug.Log("connected to contract: " + pkNftContract.address);
        Debug.Log("connected to contract: " + pkRaceContract.address);
        Debug.Log("connected to contract: " + pkTokenContract.address);

        if (!Utils.IsWebGLBuild() || chainId == 1337)
        {
            walletNFTs = LOCAL_NFT_LIST;
            nftsLoadedEvent.Invoke(walletNFTs);
        }

        uiPopup.PlayOut();
    }

    public void OpenTelegramLink()
    {
        string url = "https://t.me/partykartsio";
        Application.OpenURL(url);
    }

    public void OpenTwitterLink()
    {
        string url = "https://twitter.com/partykarts_io";
        Application.OpenURL(url);
    }

    public void OpenOpenSeaLink()
    {
        string url = "https://opensea.io/collection/partykartsbsc";
        Application.OpenURL(url);
    }

    public async Task GetNFTsForPlayer()
    {
        isLoadingNFTBalance = true;
        uiPopup.PlayIn();

        // Address of the wallet to get the NFTs of
        var address = await SDK.wallet.GetAddress();
        var nfts = SDK.currentChainData.chainId == "1337" ? LOCAL_NFT_LIST : await pkNftContract.ERC721.GetOwned(address);
        Debug.Log("NFT Count: " + nfts.Count);

        walletNFTs = nfts;
        isLoadingNFTBalance = false;
        uiPopup.PlayOut();
        nftsLoadedEvent.Invoke(nfts);
    }

    public ChainData GetChainData(string chainIdentifier)
    {
        return supportedChainData.Find(x => x.identifier == chainIdentifier);
    }

    public ChainData GetCurrentChainData()
    {
        return supportedChainData.Find(x => x.identifier == chain);
    }

    public string GetCurrentChainIdentifier()
    {
        return chain;
    }

    public BigInteger ConvertEtherToWei(string valueAsString)
    {
        BigInteger incomingValue = BigInteger.Parse(valueAsString);
        return BigInteger.Multiply(incomingValue, C.ONE_ETHER);
    }

    public BigInteger ConvertWeiToEther(string valueAsString)
    {
        BigInteger incomingValue = BigInteger.Parse(valueAsString);
        return BigInteger.Divide(incomingValue, C.ONE_ETHER);
    }

    public void WalletDisconnected()
    {
        // todo - notify smart contract of disconnection if in a race lobby,
        // so the user can be refunded their entry fee

        walletDisconnectedEvent.Invoke(true);
    }

    public void WalletConnected()
    {
        walletConnectedEvent.Invoke(true);
    }


    public async void NetworkChanged()
    {
        // todo - notify smart contract of disconnection if in a race lobby,
        // so the user can be refunded their entry fee

        walletNetworkChangeEvent.Invoke(await SDK.wallet.GetChainId());
    }
}
