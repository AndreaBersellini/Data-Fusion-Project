import cv2
import numpy as np

WIDTH = 640
HEIGHT = 480

RGBA_CHANNELS = 4
DEPTH_CHANNELS = 1

# DETECT THE CENTER POSITION OF THE REFERENCE MARK IN THE IMAGE
def detectReference(image, param):

    image = np.array(image)

    if image.ndim == 2:
        gray_image = image
    else:
        gray_image = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

    # Apply Gaussian Blur to reduce noise
    blurred_image = cv2.GaussianBlur(gray_image, (9, 9), 2)

    # Detect circles using Hough Circle Transform
    circles = cv2.HoughCircles(blurred_image, 
                                cv2.HOUGH_GRADIENT, 
                                dp=1, 
                                minDist=20, 
                                param1=param, 
                                param2=30, 
                                minRadius=0, 
                                maxRadius=0)

    reference = (0, 0, 0)

    if circles is not None:
        circles = np.uint16(np.around(circles))
        
        for i in circles[0, :]:
            cv2.circle(image, (i[0], i[1]), i[2], (0, 255, 0), 2)
            
            cv2.circle(image, (i[0], i[1]), 2, (0, 0, 255), 3)
            
            reference = (i[0], i[1], i[2])
    else:
        return detectReference(image, param - 1)

    cv2.startWindowThread()
    cv2.imshow('Detected Circles', image)
    print("Press ENTER to accept the detection...")
    if cv2.waitKey() == 13:
        cv2.destroyAllWindows()
        return reference
    else:
        cv2.destroyAllWindows()
        raise ValueError("Try to modify the parameter!")

def distanceCalibration(calib_rgb_0, calib_rgb_1, calib_image_0, calib_image_1, calib__distance_0, calib__distance_1, detect_param):
    x0, y0, _ = detectReference(calib_rgb_0, detect_param)
    x1, y1, _ = detectReference(calib_rgb_1, detect_param)

    value0 = calib_image_0.getpixel((x0, y0))
    value1 = calib_image_1.getpixel((x1, y1))

    m = (value1 - value0) / (calib__distance_1 - calib__distance_0)

    dist0 = (0 - (value1 - m * calib__distance_1)) / m
    dist255 = (255 - (value1 - m * calib__distance_1)) / m

    print(dist0, dist255)
    return (dist0, dist255) # RETURN THE DISTANCES ASSOCIATED WITH VALUES 0 AND 255

# CALCULATE THE DISTANCE FROM THE SENSOR OF A POINT IN THE IMAGE
def calculateDistance(image, position: tuple, calibration: tuple):
    x, y = position
    dist0, dist255 = calibration
    
    val = image.getpixel((x, y))
    distance = dist0 + (val * (dist255 - dist0)) / 255

    return distance

def scaleCalibration(calib_rgb, calib_depth, detect_param):
    _, _, color_radius = detectReference(calib_rgb, detect_param)
    _, _, depth_radius = detectReference(calib_depth, detect_param)

    scale_factor = depth_radius / color_radius

    print(scale_factor)
    return scale_factor
