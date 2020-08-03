using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WebCameraGrabber : MonoBehaviour {
    private WebCamTexture myWebcamTexture;
    private Texture2D texture;
    private AudioSource audio1;
    private AudioSource audio2;

    private Material cubeMat;
    private Material sphereMat;
    private Material planeMat;

    private List<Texture2D> textures = new List<Texture2D>();

    private int starttime;
    private int now;
    private int duration;

    private string mode = "recording";
    private int counter = 0;
    private int playType = 0;

    private bool pinpong = true;

    private int recInterval = 1000;

    // Use this for initialization
    void Start() {
        WebCamDevice[] WebCamdevices = WebCamTexture.devices;
        audio1 = GetComponent<AudioSource>();
        audio2 = GetComponent<AudioSource>();
        if (WebCamdevices.Length > 0) {
            myWebcamTexture = new WebCamTexture(WebCamdevices[0].name);
//            myWebcamTexture.Play();

            texture = new Texture2D(myWebcamTexture.width, myWebcamTexture.height, TextureFormat.ARGB32, false);
            planeMat = GameObject.Find("Plane 1").GetComponent<Renderer>().material;
            planeMat.mainTexture = texture;

            starttime = DateTime.Now.Hour * 60 * 60 * 1000 + DateTime.Now.Minute * 60 * 1000 +
                        DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;

            textures.Add(new Texture2D(myWebcamTexture.width, myWebcamTexture.height, TextureFormat.ARGB32, false));
            textures.Add(new Texture2D(myWebcamTexture.width, myWebcamTexture.height, TextureFormat.ARGB32, false));
            textures.Add(new Texture2D(myWebcamTexture.width, myWebcamTexture.height, TextureFormat.ARGB32, false));
            textures.Add(new Texture2D(myWebcamTexture.width, myWebcamTexture.height, TextureFormat.ARGB32, false));
        } else {
            Debug.LogError("Webカメラが検出できませんでした。");
        }
    }


    // Update is called once per frame
    void FixedUpdate() {
        now = DateTime.Now.Hour * 60 * 60 * 1000 + DateTime.Now.Minute * 60 * 1000 + DateTime.Now.Second * 1000 +
              DateTime.Now.Millisecond;
        duration = now - starttime;

        if (mode == "recording" && duration >= recInterval) {
            counter++;
            if (counter <= 3) {
                recording();
            }
            if (counter >= 4) {
                counter = 0;
                playType = UnityEngine.Random.Range(0f, 1f) < 0.5 ? 0 : 1;
                mode = "playing";
                playing();
            }
        }

        if (mode == "playing" && duration >= recInterval / 4.5) {
            counter++;
            if (counter <= 16) {
                playing();
            } else {
                counter = 0;
                planeMat.mainTexture = texture;
                mode = "recording";

                recording();
            }
        }
    }

    private void recording() {
        playSound();
        starttime = DateTime.Now.Hour * 60 * 60 * 1000 + DateTime.Now.Minute * 60 * 1000 + DateTime.Now.Second * 1000 +
                    DateTime.Now.Millisecond;

        texture.SetPixels32(myWebcamTexture.GetPixels32());
        texture.Apply();
        textures[counter].SetPixels32(myWebcamTexture.GetPixels32());
        textures[counter].Apply();
    }

    private void playing() {
        playSound();
        starttime = DateTime.Now.Hour * 60 * 60 * 1000 + DateTime.Now.Minute * 60 * 1000 +
                    DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;

        Texture currentTexture;
        if (playType == 0) {
            currentTexture = textures[counter % 4];
        } else {
            currentTexture = textures[(counter % 2) * 2];
        }

//        cubeMat.mainTexture = currentTexture;
//        sphereMat.mainTexture = currentTexture;
        planeMat.mainTexture = currentTexture;
    }

    private void playSound() {
        pinpong = !pinpong;
        if(pinpong)
            audio1.PlayOneShot(audio1.clip);
        else
            audio2.PlayOneShot(audio2.clip);
        
    }
}