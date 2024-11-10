# Data-Fusion-Project

utils.py - Contiene le costanti (es. image size) e le funzioni per calibrare e riconoscere i punti.

kinect_image_conversion.py - Converte le immagini "raw" del kinect da formato .txt a .png

fusion.ipynb (jupiter notebook) - Contiene la struttura principale dell'applicazione

Directory KinectAcquisition - Progetto C# per interagire con il sensore kinect e acquisire le immagini in formato .txt

Directory Kinect_Images_Measured - Contiene le raw images acquisite dal kinect (sia depth che rgb) e un file di misurazioni (ground truth) effettuate manualmente per calibrare e testare l'applicazione.

Directory Images - Contiene le immagini per la calibrazione, per il testing e i risultati finali con le misurazioni predette.

La struttura del main (fusion.ipynb) è la seguente:

Vengono convertite le immagini da formato .txt a .png, in seguito viene dedotto il fattore di scala misurando il raggio del riferimento nell'immagine rgb e depth_map (intrinsic calibration) e le immagini vengino ridimensionate per ottenere una corrispondenza dei punti di riferimento.

In seguito si procede con la calibrazione dei toni di grigio delle immagini depth_map (extrinsic calibration) per stabilire a che distaza corrisponde il valore 0 e 255, questo avviene misurando le distanze dei riferimenti delle immagini di calibrazione (assumendo un mapping lineare tra distanze e valori rgb) e calcolando i valori estrami in proporzione.

Come ultimo passaggio, le immagini di testing vengono elaborate e viene detotta la distanza di ogni riferimento a partire dai dati di calibrazione.

[La funzione di detection delle reference utilizza la libreria cv2 di python per localizzare cerchi nelle immagini]
