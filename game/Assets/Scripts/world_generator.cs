﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class world_generator : MonoBehaviour
{
    const float kTileWidth = 2f;
    Vector3 kPlaneScale = Vector3.one * 0.2f;
    Vector3 kBallStartPosition = new Vector3(0, -0.089688f, -0.95f);

    private TrafikverketClient client = new TrafikverketClient();


    List<Stop> stops = new List<Stop>();

    public GameObject levelRootPrefab;
    public GameObject ballPreset;

    public int tiles = 1;

    // Start is called before the first frame update
    async void Start()
    {
        var stationInformations = (await client.GetStationInformations("2016"))
            .Where(s => s.EstimatedTime > System.DateTime.Now)
            .ToList();

        var numberOfStops = stationInformations.Count * 2 - 1;

        for (var i = 0; i < numberOfStops; i++) {
            var stop = new Stop();
            var index = i / 2;
            if(i % 2 == 0)
            {
                stop.PublicName = stationInformations[index].Name;
                stop.ArrivalTime = stationInformations[index].EstimatedTime;
                stop.Type = StopType.City;
            } else {
                stop.PublicName = "Forest";

                stop.ArrivalTime = stationInformations[index].EstimatedTime
                    .AddMinutes((stationInformations[index + 1].EstimatedTime - 
                        stationInformations[index].EstimatedTime).TotalMinutes/2);

                stop.Type = StopType.Forest;
            }

            stop.Position = new Vector3(0, 0, i * kTileWidth);
            stop.Order = i;
            stops.Add(stop);

        }
        foreach (var stop in stops) {
            Debug.Log(stop.Name);
            var levelRoot = Object.Instantiate(levelRootPrefab, stop.Position, Quaternion.identity);
            levelRoot.transform.SetParent(transform);
            levelRoot.name = stop.Name;
            var renderer = levelRoot.transform.GetChild(0).GetComponent<MeshRenderer>();
            renderer.material.color = Random.ColorHSV();
            var levelInformation = levelRoot.transform.GetChild(0).GetComponent<LevelInformation>();
            levelInformation.IsActive = stop.Order == 0;
            levelInformation.Order = stop.Order;
        }
        
        // Right plane
        var rightPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        rightPlane.transform.position = new Vector3(kTileWidth * (tiles + 1) / 2, 0, kTileWidth * (tiles - 1f) / 2);
        rightPlane.transform.localScale = tiles * kPlaneScale;
        rightPlane.transform.SetParent(transform);
        // Left plane
        var leftPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        leftPlane.transform.position = new Vector3(kTileWidth * (-tiles - 1) / 2, 0, kTileWidth * (tiles - 1f) / 2);
        leftPlane.transform.localScale = tiles * kPlaneScale;
        leftPlane.transform.SetParent(transform);
        // ...and beyond
        var beyondPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        beyondPlane.transform.position = new Vector3(0, 0, 2 * kTileWidth * tiles);
        beyondPlane.transform.localScale = 2 * (tiles + 0.5f) * kPlaneScale;
        beyondPlane.transform.SetParent(transform);

        var mahBallz = Object.Instantiate(ballPreset, kBallStartPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
