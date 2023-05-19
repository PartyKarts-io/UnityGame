using System.Collections;
using System.Collections.Generic;
using Thirdweb;
using UnityEngine;
using Michsky.UI.Reach;
using System.Linq;

public class CarSelectorInLobby : MonoBehaviour
{
    [SerializeField] GameObject CarOptionButtonList;

    void OnEnable()
    {
        List<NFT> nfts = ThirdwebManager.Instance.walletNFTs;
        List<string> nftNames = nfts.Select((n => n.metadata.name)).Distinct().ToList();
        CarOptionButtonList.GetComponentsInChildren<ButtonManager>().ToList().ForEach(b =>
        {
            b.Interactable(nftNames.Contains(b.name));
            b.UpdateUI();
        });
    }
}
