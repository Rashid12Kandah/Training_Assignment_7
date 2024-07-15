# Dilation and Erosion Assignment

csc -unsafe -out:Dilation_Erosion.exe /main:BorderRemoval.ImageProcessing Dilation_Erosion.cs Task_4.cs Color_To_GS_Conv.cs

mono Dilation_Erosion.exe <method> ("dilate","erode") <path/to/img>

## Kernel used in the example

{[0,  250, 0 ],
 [250,250,250],
 [0,  250, 0 ]}

## Orignal Image - 24-bit Image 

<img src="https://github.com/Rashid12Kandah/Training_Assignment_7/blob/main/Dame1.jpg" alt="24-bit coloured Image of flower orchard" width="240" height="360">

### The image is transformed into 8-bit grayscaleusing the the 3rd assignment code

<img src="https://github.com/Rashid12Kandah/Training_Assignment_7/blob/main/Dame1.jpeg" alt="8-bit grayscale image of flower orchard" width="240" height="360">

## Dilation Output

<img src="https://github.com/Rashid12Kandah/Training_Assignment_7/blob/main/dilate.png" alt="8-bit grayscale dilated image of flower orchard" width="240" height="360">

## Erosion Output

<img src="https://github.com/Rashid12Kandah/Training_Assignment_7/blob/main/erode.png" alt="8-bit grayscale eroded image of flower orchard" width="240" height="360">
