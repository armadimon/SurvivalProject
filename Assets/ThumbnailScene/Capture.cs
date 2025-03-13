using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public enum Grade
{
    None,
    Normal,
    Uncommon,
    Rare,
    Legend,
}

public enum Size
{
    POT64,
    POT128,
    POT256,
    POT512,
    POT1024,
}
public class Capture : MonoBehaviour
{
    public Camera cam;
    public RenderTexture rt;
    public Image bg;
    public Grade grade;
    public Size size;

    public GameObject[] obj;
    int index = 0;

    private void Start()
    {
        cam = Camera.main;
        SettingColor();
        SettingSzie();

    }

    public void Create()
    {
        StartCoroutine(CapturerImage());   
    }

    public void AllCreate()
    {
        StartCoroutine(AllCapturerImage());
    }


    IEnumerator CapturerImage()
    {
        yield return null;
        
        Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, true);
        RenderTexture.active = rt;
        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        yield return null;

        var data = texture.EncodeToPNG();
        string name = "Thnumbnail";
        string extention = ".png";
        string path = Application.persistentDataPath + "/Thnumbnail/";

        Debug.Log(path);

        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        File.WriteAllBytes(path + name + extention, data);

        yield return null;

    }

    IEnumerator AllCapturerImage()
    {
        while (index < obj.Length)
        {
            var nowobj = Instantiate(obj[index].gameObject);

            yield return null;

            Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, true);
            RenderTexture.active = rt;
            texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

            yield return null;

            var data = texture.EncodeToPNG();
            string name = $"Thnumbnail_{obj[index].gameObject.name}";
            string extention = ".png";
            string path = Application.persistentDataPath + "/Thnumbnail/";

            Debug.Log(path);

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            File.WriteAllBytes(path + name + extention, data);

            yield return null;

            DestroyImmediate(nowobj);
            index++;

            yield return null;
        }
    }
    void SettingColor()
    {
        switch (grade)
        {
            case Grade.None:
                cam.backgroundColor = new Color(0, 0, 0, 0); // 카메라 배경색을 투명으로 설정
                bg.color = new Color(0, 0, 0, 0); // 배경 이미지 색을 투명으로 설정
                break;
            case Grade.Normal:
                cam.backgroundColor = Color.white;
                bg.color = Color.white; break;
            case Grade.Uncommon:
                cam.backgroundColor = Color.green;
                bg.color = Color.green; break;
            case Grade.Rare:
                cam.backgroundColor = Color.blue;
                bg.color = Color.blue; break;
            case Grade.Legend:
                cam.backgroundColor = Color.yellow;
                bg.color = Color.yellow; break;
            default:
                break;
        }
    }

    void SettingSzie()
    {
        switch (size)
        {
            case Size.POT64:
                rt.width = 64;
                rt.height = 64;
                break;
            case Size.POT128:
                rt.width = 128;
                rt.height = 128;
                break;
            case Size.POT256:
                rt.width = 256;
                rt.height = 256;
                break;
            case Size.POT512:
                rt.width = 512;
                rt.height = 512;
                break;
            case Size.POT1024:
                rt.width = 1024;
                rt.height = 1024;
                break;
            default:
                break;
        }
    }
}
