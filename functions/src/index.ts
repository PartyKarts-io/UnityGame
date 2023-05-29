/**
 * Import function triggers from their respective submodules:
 *
 * import {onCall} from "firebase-functions/v2/https";
 * import {onDocumentWritten} from "firebase-functions/v2/firestore";
 *
 * See a full list of supported triggers at https://firebase.google.com/docs/functions
 */
import { onRequest } from "firebase-functions/v2/https";
import * as logger from "firebase-functions/logger";
import { Markup, Telegraf } from 'telegraf';

// Start writing functions
// https://firebase.google.com/docs/functions/typescript


export const sendRaceCreatedMessage = onRequest(async (req, res) => {
    logger.info("A Race Has Been Created!", { structuredData: true });
    try {
        const chatId = '-1001924144786';
        const botToken = '6293735263:AAEvbz-x_dB7NJsrGTYia2K5jYKEDsF5c_Y';
        const bot = new Telegraf(botToken);

        const serverRegion: string = req.query.serverRegion as string;
        const entryFee: string = req.query.entryFee as string;
        const trackName: string = req.query.trackName as string;

        console.log(serverRegion);
        console.log(entryFee);
        console.log(trackName);

        const message = `
🏁🏁🏎🏎🏎🏎🏎🏎🏎🏎🏎🏎🏎🏁🏁
<b>A new race lobby has been created!</b>

<b>Lobby Details:</b>

<b>Server Region:</b> ${getServerFromKey(serverRegion)}
<b>Entry Fee:</b> ${formatFee(entryFee)}

GO TAKE THEIR MONEY!
🏁🏁🏎🏎🏎🏎🏎🏎🏎🏎🏎🏎🏎🏁🏁
`

        // Button
        const buttonText = 'Let\'s Race!';
        const buttonUrl = 'https://partykarts.io/play';

        // Image
        const images = [
            'https://uploads-ssl.webflow.com/6441c70b5f87c4179380b545/6461ab2552fe54afa425b62b_PK_BANNER.png',
        ]

        if (!chatId) {
            throw new Error('Missing chatId');
        }

        if (!botToken) {
            throw new Error('Missing botToken');
        }

        const replyMarkup = Markup.inlineKeyboard([
            Markup.button.url(buttonText, buttonUrl),
        ]).reply_markup

        await bot.telegram.sendPhoto(chatId, { url: images[0] }, {
            caption: message,
            reply_markup: replyMarkup,
            parse_mode: 'HTML',
        });

        res.status(200).send('Message sent successfully');
    } catch (error) {
        console.error('Error sending message:', error);
        res.status(500).send('An error occurred');
    }
});

// function getTrackNameFromKey(key: string) {
//     switch (key) {
//         case "RaceTrackRace":
//             return "Party Park";
//         case "IcelandFestivalRace":
//             return "Party in Iceland";
//         default:
//             return "Party Park";
//     }
// }

function getServerFromKey(key: string) {
    switch (key) {
        case "usw":
            return "US West";
        case "us":
            return "US East";
        case "eu":
            return "Europe";
        case "in":
            return "India";
        case "ru":
            return "Russia";
        case "sa":
            return "South America";
        case "kr":
            return "South Korea";
        case "au":
            return "Australia";
        case "cae":
            return "Canada East";
        case "cn":
            return "China";
        case "jp":
            return "Japan";
        default:
            return "US West";
    }
}

function formatFee(fee: string) {
    switch (fee) {
        case "1":
            return "1 $KART";
        case "5000":
            return "5k $KART";
        case "10000":
            return "10k $KART";
        case "20000":
            return "20k $KART";
        case "50000":
            return "50k $KART";
        case "100000":
            return "100k $KART";
        case "150000":
            return "150k $KART";
        case "250000":
            return "250k $KART";
        case "500000":
            return "500k $KART";
        default:
            return "1 $KART";
    }
}
