import {appWindow, Monitor, PhysicalPosition, PhysicalSize} from '@tauri-apps/api/window';

export class ScreenshotWrapper {
    public constructor(
        private readonly _element: HTMLElement,
    ) {
    }

    public async displayScreenshots(screenshots: Map<string, HTMLImageElement>, monitors: Monitor[]): Promise<void> {
        const xMin = Math.min(...monitors.map(m => m.position.x))
        const yMin = Math.min(...monitors.map(m => m.position.y))
        const xMax = Math.min(...monitors.map(m => m.position.x + m.size.width))
        const yMax = Math.min(...monitors.map(m => m.position.y + m.size.height))

        await appWindow.setPosition(new PhysicalPosition(xMin, yMin));
        await appWindow.setSize(new PhysicalSize(xMax - xMin, yMax - yMin));

        const elements = [];

        for (const monitor of monitors) {
            if (monitor.name === null) continue;

            const screenshot = screenshots.get(monitor.name);

            // TODO: Handle non existing screenshot for that monitor
            if (screenshot === undefined) continue;

            screenshot.style.left = monitor.position.x + 'px';
            screenshot.style.top = monitor.position.y + 'px';

            elements.push(screenshot)
        }

        this._element.append(...elements)
    }

    public get element(): HTMLElement {
        return this._element;
    }
}