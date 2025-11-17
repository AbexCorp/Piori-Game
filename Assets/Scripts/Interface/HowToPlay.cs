using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _text;
    [SerializeField]
    private Image _image;
    [SerializeField]
    private string[] _informationList;
    [SerializeField]
    private Sprite[] _imageList;

    private int _counter = 0;

    private void Start()
    {
        _text.text = _informationList[_counter];
        SetImage(_imageList[_counter]);
    }

    public void MoveCounter(int value)
    {
        _counter += value;
        _counter = Mathf.Clamp(_counter, 0, _informationList.Length - 1);

        _text.text = _informationList[_counter];
        SetImage(_imageList[_counter]);
    }
    private void SetImage(Sprite sprite)
    {
        _image.sprite = _imageList[_counter];
        if (sprite == null)
            _image.color = Color.clear;
        else
            _image.color = Color.white;
    }
}
