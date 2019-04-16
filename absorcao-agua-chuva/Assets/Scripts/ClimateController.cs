﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ClimateController : MonoBehaviour{

    [SerializeField] private GameObject[] rains;
    [SerializeField] private Transform river;
    [SerializeField] private Text rainfallIndexText;
    [SerializeField] private Text riverHeightText;
    [SerializeField] private Terrain terrain;
    private int rainLevel;
    private float minPosition;
    private float maxPosition;
    private float speed = 1.0f;
    private float rainfallIndex = 0.0f;
    private float range = 40.0f;

    private float riverHeightCountMax = 13.0f;
    private float rainfallIndexCountMax = 350.0f;
    private float rainfallInc = 0.5f;
    private float rainfallSpeed = 30f;

    private void Start() {

        switch(MenuManager.terrainType) {
            case TerrainType.PERMEAVEL:
                range = 20;
                riverHeightCountMax = 4.0f;
                break;
            case TerrainType.SEMI_PERMEAVEL:
                range = 35;
                riverHeightCountMax = 8.0f;
                break;
            case TerrainType.IMPERMEAVEL:
                range = 50;
                riverHeightCountMax = 16.0f;
                break;
        }

        minPosition = river.position.y;
        maxPosition = river.position.y + range;

        TerrainUtils terrainUtils = new TerrainUtils(terrain.terrainData);
        terrainUtils.SetTerrainTexture(MenuManager.terrainType);
    }

    public void StartRain(int rainIntensity) {
        rains[rainLevel].SetActive(false);
        rainLevel = rainIntensity;

        switch(rainLevel) {
            case 0:
                speed = 1.0f;
                rainfallInc = 0.5f;
                break;
            case 1:
                speed = 3.0f;
                rainfallInc = 1.5f;
                break;
            case 2:
                speed = 5.0f;
                rainfallInc = 3.0f;
                break;
        }

        StartCoroutine(MoveRiverUp());
    }
    
    public void StartDry() {
        StartCoroutine(MoveRiverDown());
    }

    public void StopRain() {
        StopAllCoroutines();
        rains[rainLevel].SetActive(false);
    }

    private IEnumerator MoveRiverUp () {
        if(river.position.y < maxPosition)
            rains[rainLevel].SetActive(true);
        
        yield return new WaitForSeconds(2f);
        while (river.position.y < maxPosition) {
            river.position = new Vector3(river.position.x, 
                river.position.y + Time.deltaTime * speed, river.position.z);

            UpdateRiverHeightText();
            UpdateRainfallIndexText();

            yield return null;
        }
        rains[rainLevel].SetActive(false);
    }

    private IEnumerator MoveRiverDown() {
        rains[rainLevel].SetActive(false);
        yield return new WaitForSeconds(.5f);
        while (river.position.y > minPosition) {
            river.position = new Vector3(river.position.x, 
                river.position.y - Time.deltaTime * speed, river.position.z);

            UpdateRiverHeightText();
            yield return null;
        }
    }

    private void UpdateRiverHeightText() {
        float riverHeight = (river.position.y * riverHeightCountMax) / range;
        riverHeightText.text = String.Format("{0:0.0}", riverHeight+1) + "m";
    }

    private void UpdateRainfallIndexText() {
        rainfallIndex += Time.deltaTime * rainfallInc * rainfallSpeed;
        float rainfall = (river.position.y * rainfallIndexCountMax) / range;
        rainfallIndexText.text = String.Format("{0:0.0}", rainfall) + "mm";
    }
}