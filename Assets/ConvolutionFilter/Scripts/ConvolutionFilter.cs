// https://github.com/unitycoder/ConvolutionFilter
// based on information from http://setosa.io/ev/image-kernels/

using UnityEngine;

public class ConvolutionFilter : MonoBehaviour
{
    public Material sourceMat;
    public Material targetMat;
    public Material overlayMat;

    void Start()
    {
        // get source pixels
        var sourceTex = ((Texture2D)sourceMat.mainTexture);
        var sourcePixels = sourceTex.GetPixels(0);


        // prepare target texture and array
        var targetPixels = sourceTex.GetPixels(0); // copy of
        var targetTex = new Texture2D(sourceTex.width, sourceTex.height, TextureFormat.RGB24, false);
        targetTex.SetPixels(sourcePixels, 0);
        targetTex.Apply(false);

        // show it on our materials
        targetMat.mainTexture = targetTex;

        // overlay is just showing results multiplied original texture and generated texture
        overlayMat.mainTexture = sourceTex;
        overlayMat.SetTexture("_Tex2", targetTex);

        // kernels
        float[] kernel = new float[] { 0, 0, 0,
                                       0, 1, 0,
                                       0, 0, 0};
        // index                       0  1  2
        //                             3  4  5
        //                             6  7  8

        // premade filters:
        // blur
        //kernel = new float[] { 0.0625f, 0.125f, 0.0625f, 0.125f, 0.25f, 0.125f, 0.0625f, 0.125f, 0.0625f };
        // emboss
        kernel = new float[] { -2, -1, 0, -1, 1, 1, 0, 1, 2 };
        // outline
        //kernel = new float[] { -1, -1, -1, -1, 8, -1, -1, -1, -1 };
        // sharpen
        //kernel = new float[] { 0, -1, 0, -1, 5, -1, 0, -1, 0 };

        // for quickly testing different values
        float kernelMultiplier = 1f;

        // loop all pixels (except borders)
        int width = sourceTex.width;
        int height = sourceTex.height;
        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                int ci = x * width + y;

                int top = x * width + y - 1;
                int bottom = x * width + y + 1;
                int left = (x - 1) * width + y;
                int right = (x + 1) * width + y;

                int topLeft = (x - 1) * width + y - 1;
                int topRight = (x + 1) * width + y - 1;
                int bottomLeft = (x - 1) * width + y + 1;
                int bottomRight = (x + 1) * width + y + 1;

                float pCenter = (sourcePixels[ci].r * kernel[4] * kernelMultiplier);
                float pTop = (sourcePixels[top].r * kernel[1] * kernelMultiplier);
                float pBottom = (sourcePixels[bottom].r * kernel[7] * kernelMultiplier);
                float pLeft = (sourcePixels[left].r * kernel[3] * kernelMultiplier);
                float pRight = (sourcePixels[right].r * kernel[5] * kernelMultiplier);
                float pTopRight = (sourcePixels[topRight].r * kernel[2] * kernelMultiplier);
                float pTopLeft = (sourcePixels[topLeft].r * kernel[0] * kernelMultiplier);
                float pBottomRight = (sourcePixels[bottomRight].r * kernel[8] * kernelMultiplier);
                float pBottomLeft = (sourcePixels[bottomLeft].r * kernel[6] * kernelMultiplier);

                float output = (pCenter + pTop + pBottom + pLeft + pRight + pTopRight + pTopLeft + pBottomRight + pBottomLeft);

                targetPixels[ci] = new Color(output, output, output, 1);
            }
        }

        // apply new pixels
        targetTex.SetPixels(targetPixels, 0);
        targetTex.Apply(false);
    }
}
