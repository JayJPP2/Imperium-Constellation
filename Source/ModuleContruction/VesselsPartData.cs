using UnityEngine;
using UnityEditor;


public class VesselsPartData
{
    public int Cost { get; private set; }

    public float Speed { get; private set; }

    public string Color { get; private set; }

    public VesselsPartData()
    {
        this.Cost = 0;
        this.Speed = 0.0f;
        this.Color = "White";
    }

    public VesselsPartData(int cost, float speed, string color)
    {
        this.Cost = cost;
        this.Speed = speed;
        this.Color = color;
    }
}