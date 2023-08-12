
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator : MonoBehaviour {
    
    [SerializeField]
    private GameObject field;

    [SerializeField]
    private Material noiseMat;

    [SerializeField]
    private int blurRadius;
    [SerializeField]
    private int blurIterations;

    [SerializeField]
    private float beforeLerpWaitTime = 1.0f;

    [SerializeField]
    private float lerpSpeed;

    [SerializeField]
    private TerrainGenerator terrainGenerator;

    private float avgR, avgG, avgB = 0;
    private float blurPixelCount = 0;

    public IEnumerator GenerateTexture(GameObject[,] _cells) {

        Texture2D texture = new Texture2D(_cells.GetLength(0), _cells.GetLength(1));
        texture.filterMode = FilterMode.Point;

        for(int x = 0; x < _cells.GetLength(0); x++) {
            for(int y = 0; y < _cells.GetLength(1); y++) {
                if(_cells[x, y] == null) {
                    texture.SetPixel(x, y, Color.black);
                }
                else {
                    texture.SetPixel(x, y, Color.white);
                }
            }
        }
        texture.Apply();
        noiseMat.SetTexture("_Texture", texture);

        yield return new WaitForEndOfFrame();

        Texture2D blurTexture = new Texture2D(_cells.GetLength(0) * 5, _cells.GetLength(1) * 5);
        blurTexture.filterMode = FilterMode.Point;

        for(int x = 0; x < blurTexture.width; x++) {
            for(int y = 0; y < blurTexture.height; y++) {
                if(_cells[Mathf.FloorToInt(x/5), Mathf.FloorToInt(y/5)] == null) {
                    blurTexture.SetPixel(x, y, Color.black);
                }
                else {
                    blurTexture.SetPixel(x, y, Color.white);
                }
            }
        }
        blurTexture = GenerateBlurTexture(blurTexture);
        noiseMat.SetTexture("_BlurTexture", blurTexture);

        field.GetComponent<MeshRenderer>().material = noiseMat;

        noiseMat.SetFloat("_MixAmount", 0.0f);

        yield return new WaitForSeconds(beforeLerpWaitTime);

        float lerpValue = 0.0f;

        while(lerpValue < 1.0f) {
            noiseMat.SetFloat("_MixAmount", lerpValue);
            lerpValue += lerpSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        noiseMat.SetFloat("_MixAmount", 1.0f);

        field.gameObject.SetActive(false);

        terrainGenerator.StartCoroutine(terrainGenerator.GenerateTerrain(blurTexture));

    }

    private Texture2D GenerateBlurTexture(Texture2D _image){
        Texture2D tex = _image;
       
        for (var i = 0; i < blurIterations; i++) {
           
            tex = BlurImage(tex, blurRadius, true);
            tex = BlurImage(tex, blurRadius, false);
           
        }
       
        return tex;
    }
   
    private Texture2D BlurImage(Texture2D _image, int _blurSize, bool _horizontal){
       
        Texture2D blurred = new Texture2D(_image.width, _image.height);
        int _W = _image.width;
        int _H = _image.height;
        int xx, yy, x, y;
       
        if (_horizontal) {
            for (yy = 0; yy < _H; yy++) {
                for (xx = 0; xx < _W; xx++) {
                    ResetPixel();
                   
                    //Right side of pixel
                   
                    for (x = xx; (x < xx + _blurSize && x < _W); x++) {
                        AddPixel(_image.GetPixel(x, yy));
                    }
                   
                    //Left side of pixel
                   
                    for (x = xx; (x > xx - _blurSize && x > 0); x--) {
                        AddPixel(_image.GetPixel(x, yy));
                       
                    }
                   
                   
                    CalcPixel();
                   
                    for (x = xx; x < xx + _blurSize && x < _W; x++) {
                        blurred.SetPixel(x, yy, new Color(avgR, avgG, avgB, 1.0f));
                       
                    }
                }
            }
        }
       
        else {
            for (xx = 0; xx < _W; xx++) {
                for (yy = 0; yy < _H; yy++) {
                    ResetPixel();
                   
                    //Over pixel
                   
                    for (y = yy; (y < yy + _blurSize && y < _H); y++) {
                        AddPixel(_image.GetPixel(xx, y));
                    }
                    //Under pixel
                   
                    for (y = yy; (y > yy - _blurSize && y > 0); y--) {
                        AddPixel(_image.GetPixel(xx, y));
                    }
                    CalcPixel();
                    for (y = yy; y < yy + _blurSize && y < _H; y++) {
                        blurred.SetPixel(xx, y, new Color(avgR, avgG, avgB, 1.0f));
                       
                    }
                }
            }
        }
       
        blurred.Apply();
        return blurred;

    }

    private void AddPixel(Color _pixel) {
        avgR += _pixel.r;
        avgG += _pixel.g;
        avgB += _pixel.b;
        blurPixelCount++;
    }

    private void ResetPixel() {
        avgR = 0.0f;
        avgG = 0.0f;
        avgB = 0.0f;
        blurPixelCount = 0;
    }

    private void CalcPixel() {
        avgR = avgR / blurPixelCount;
        avgG = avgG / blurPixelCount;
        avgB = avgB / blurPixelCount;
    }

}