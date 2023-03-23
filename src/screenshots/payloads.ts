interface ScreenshotPayload {
    name: string;
    image: string;
}

interface ScreenshotCapturedPayload {
    screenshots: ScreenshotPayload[];
}