import {invoke} from "@tauri-apps/api";

interface ScreenshotCapturedPayload {
    images: string[]
}

const payload = await invoke<ScreenshotCapturedPayload>('capture_screenshots')

document.body.classList.remove('not-ready')

for (const base64 of payload.images) {
    const img = document.createElement("img")

    img.classList.add('screenshot')
    img.src = `data:image/bmp;base64,${base64}`

    document.body.appendChild(img)
}