import os
import numpy as np
import matplotlib.pyplot as plt
from utils import *

def generateKinectDataset():
    RAW_IMG_DIR = './Kinecet_Images_Measured'
    files_num = len([name for name in os.listdir(RAW_IMG_DIR) if os.path.isfile(os.path.join(RAW_IMG_DIR, name))])

    for i in range((files_num - 1) // 2):
        rgb_path = RAW_IMG_DIR + "/rgb_" + str(i) + ".txt"
        with open(rgb_path, "r") as file:
            data = [int(line.strip()) for line in file]

        rgb_image = np.array(data, dtype=np.uint8)
        rgb_image = rgb_image.reshape((HEIGHT, WIDTH, RGBA_CHANNELS))
        rgb_image = rgb_image[:, :, :3]

        if i < 2:
            save_path = "Images/Calibration/rgb_image_" + str(i) + ".png"
        else:
            save_path = "Images/rgb_image_" + str(i) + ".png"
        plt.imsave(save_path, rgb_image)

        depth_path = RAW_IMG_DIR + "/depth_" + str(i) + ".txt"
        with open(depth_path, "r") as file:
            data = [int(line.strip()) for line in file]

        depth_image = np.array(data)
        min_value = np.min(depth_image)
        max_value = np.max(depth_image)

        if max_value > min_value:
            normalized_data = (depth_image - min_value) / (max_value - min_value) * 255
        else:
            normalized_data = np.zeros_like(depth_image)

        depth_image = normalized_data.astype(np.uint8)
        depth_image = depth_image.reshape((HEIGHT, WIDTH))

        if i < 2:
            save_path = "Images/Calibration/depth_image_" + str(i) + ".png"
        else:
            save_path = "Images/depth_image_" + str(i) + ".png"
        plt.imsave(save_path, depth_image, cmap='gray')