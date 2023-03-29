import {Event, listen} from '@tauri-apps/api/event';
import {LiveShot} from '../main';

const SCREENSHOT_CLASS: string = 'screenshot';

export class ScreenshotListener {
    public constructor(
        private readonly _resolveScreenshotsEvent: string,
    ) {
    }

    public async listen(): Promise<void> {
        const cb = this._onResolveScreenshots.bind(this);

        await listen<ScreenshotCapturedPayload>(
            this._resolveScreenshotsEvent,
            cb
        );
    }

    private _buildScreenshots(payload: ScreenshotCapturedPayload): Map<string, HTMLImageElement> {
        const images: Map<string, HTMLImageElement> = new Map();

        for (const {name, image} of payload.screenshots) {
            const imageElement = document.createElement('img');

            imageElement.classList.add(SCREENSHOT_CLASS);
            imageElement.src = `data:image/bmp;base64,${image}`;

            images.set(name, imageElement);
        }

        return images;
    }

    private async _onResolveScreenshots(event: Event<ScreenshotCapturedPayload>): Promise<void> {
        const screenshots = this._buildScreenshots(event.payload);

        await LiveShot.Instance.displayScreenshots(screenshots)
    }

}