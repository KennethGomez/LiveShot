import {listen} from "@tauri-apps/api/event";
import {invoke} from "@tauri-apps/api";

interface ScreenshotCapturedPayload {
    images: string[]
}

// Listen only once for screenshot captured event
const unlisten = await listen<ScreenshotCapturedPayload>(
    'screenshot-captured',
    (event) => {
        for (const base64 of event.payload.images) {
            const img = document.createElement("img")

            img.src = `data:image/bmp;base64,${base64}`

            document.body.appendChild(img)
        }

        // Stop listening event after catched
        unlisten();

        // Open window
        invoke('show_window')
    }
)