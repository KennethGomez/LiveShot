import {appWindow, availableMonitors} from '@tauri-apps/api/window';
import {ScreenshotListener} from './screenshots/loader';
import {KeyboardEvents} from './keyboard/events';
import {ScreenshotWrapper} from './screenshots/wrapper';

const NOT_READY_CLASS: string = 'not-ready';
const EXIT_KEY: string = 'Escape';

export class LiveShot {
    private readonly _screenshotListener: ScreenshotListener;
    private readonly _screenshotWrapper: ScreenshotWrapper;
    private readonly _keyboard: KeyboardEvents;

    public static Instance = new LiveShot();

    public constructor() {
        this._screenshotListener = new ScreenshotListener('screenshots');
        this._screenshotWrapper = new ScreenshotWrapper(document.body);
        this._keyboard = new KeyboardEvents();
    }

    public async init(): Promise<void> {
        await this._screenshotListener.listen();

        await this._preventClose();

        this._keyboard.registerWindowEvent(EXIT_KEY, async () => {
            this._screenshotWrapper.element.classList.add(NOT_READY_CLASS);

            await appWindow.hide();
        });

        this._keyboard.initWindowEvents();
    }

    private async _preventClose() {
        await appWindow.onCloseRequested(async (e) => {
            e.preventDefault();

            await appWindow.hide();
        });
    }

    public async displayScreenshots(screenshots: Map<string, HTMLImageElement>): Promise<void> {
        await this._screenshotWrapper.displayScreenshots(
            screenshots, await availableMonitors()
        );

        // Remove not ready class from wrapper
        this._screenshotWrapper.element.classList.remove(NOT_READY_CLASS);
    }
}

await LiveShot.Instance.init();