import {invoke} from '@tauri-apps/api';

const SCREENSHOT_CLASS: string = 'screenshot';

export class ScreenshotLoader {
    public constructor(
        private readonly _resolveScreenshotsCommand: string,
    ) {
    }

    public async load(): Promise<HTMLImageElement[]> {
        const images: HTMLImageElement[] = [];

        const payload = await invoke<ScreenshotCapturedPayload>(this._resolveScreenshotsCommand);

        for (const base64 of payload.images) {
            const imageElement = document.createElement('img');

            imageElement.classList.add(SCREENSHOT_CLASS);
            imageElement.src = `data:image/bmp;base64,${base64}`;

            images.push(imageElement)
        }

        return images;
    }

}