'''
    Screen Resolution: 1592 * 982.
    Hollow Knight window must be in the same desktop as this script.
    This script captures the Hollow Knight window and saves it as a PNG file.
'''

from Quartz import CGWindowListCopyWindowInfo, kCGWindowListOptionOnScreenOnly, kCGNullWindowID
from Quartz import CGWindowListCreateImage, CGRectInfinite, kCGWindowImageBoundsIgnoreFraming, kCGWindowImageDefault
from Quartz import CGRectMake
from PIL import Image
import Quartz.CoreGraphics as CG

def find_windows_by_title(title_substring):
    windows = CGWindowListCopyWindowInfo(kCGWindowListOptionOnScreenOnly, kCGNullWindowID)
    matched = []
    for w in windows:
        window_name = w.get('kCGWindowName', '')
        if window_name and title_substring.lower() in window_name.lower():
            matched.append(w)
    return matched

def capture_window_image(window):
    bounds = window['kCGWindowBounds']
    x = bounds['X']
    y = bounds['Y']
    width = bounds['Width']
    height = bounds['Height']

    # Quartz 坐标系 y轴从屏幕底部开始，所以需要转化：
    # 这里用CGRectMake + CGWindowListCreateImage截取窗口区域
    rect = CGRectMake(x, y, width, height)
    image_ref = CGWindowListCreateImage(rect, CG.kCGWindowListOptionIncludingWindow, window['kCGWindowNumber'], kCGWindowImageBoundsIgnoreFraming)

    # 转换成PIL图片
    width = CG.CGImageGetWidth(image_ref)
    height = CG.CGImageGetHeight(image_ref)
    bytes_per_row = CG.CGImageGetBytesPerRow(image_ref)
    data_provider = CG.CGImageGetDataProvider(image_ref)
    data = CG.CGDataProviderCopyData(data_provider)

    import io
    img = Image.frombuffer("RGBA", (width, height), data, "raw", "RGBA", bytes_per_row, 1)
    return img

if __name__ == "__main__":
    title = "hollow knight"
    windows = find_windows_by_title(title)
    if not windows:
        print(f"No windows found containing '{title}'")
    else:
        print(f"Found {len(windows)} window(s) containing '{title}'")
        for i, win in enumerate(windows):
            img = capture_window_image(win)
            filename = f"hollow_knight_window_{i}.png"
            img.save(filename)
            print(f"Saved screenshot: {filename}")
