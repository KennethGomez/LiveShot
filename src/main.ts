import {invoke} from "@tauri-apps/api";
import {appWindow} from "@tauri-apps/api/window";

interface ScreenshotCapturedPayload {
    images: string[]
}

const EXIT_KEY = 'Escape'

const payload = await invoke<ScreenshotCapturedPayload>('get_screenshots')

document.body.classList.remove('not-ready')

for (const base64 of payload.images) {
    const img = document.createElement("img")

    img.classList.add('screenshot')
    img.src = `data:image/bmp;base64,${base64}`

    document.body.appendChild(img)
}

await appWindow.setFocus()

document.addEventListener('keydown', async (event) => {
    if (event.key == EXIT_KEY) {
        await appWindow.close()
    }
})