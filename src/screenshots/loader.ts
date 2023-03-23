import {invoke} from '@tauri-apps/api';

const SCREENSHOT_CLASS: string = 'screenshot';

export class ScreenshotLoader {
    public constructor(
        private readonly _resolveScreenshotsCommand: string,
    ) {
    }

    public async load(): Promise<Map<string, HTMLImageElement>> {
        const images: Map<string, HTMLImageElement> = new Map();

        const payload = await invoke<ScreenshotCapturedPayload>(this._resolveScreenshotsCommand);

        for (const {name, image} of payload.screenshots) {
            const imageElement = document.createElement('img');

            imageElement.classList.add(SCREENSHOT_CLASS);
            imageElement.src = `data:image/bmp;base64,${image}`;

            images.set(name, imageElement)
        }

        return images;
    }

}