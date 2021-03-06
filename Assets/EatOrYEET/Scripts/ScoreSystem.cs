﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreSystem : MonoBehaviour
{
    private int _currentScore;
    private float _globalScoreMultiplierBonus;
    private Dictionary<sFood.FoodCategory, float> _foodCategoryMultiplierBonus;

    [SerializeField]
    private GameObject _totalScoreDisplay;

    // Start is called before the first frame update
    void Start()
    {
        _currentScore = 0;
        _globalScoreMultiplierBonus = 0f;

        // Set up initial multiplier bonuses for all Food Categories
        _foodCategoryMultiplierBonus = new Dictionary<sFood.FoodCategory, float>();
        sFood.FoodCategory[] foodCategories = (sFood.FoodCategory[])System.Enum.GetValues(typeof(sFood.FoodCategory));

        foreach(sFood.FoodCategory category in foodCategories){
            _foodCategoryMultiplierBonus[category] = 0f;
        }
    }

    public void AdjustScore(FoodItem food, bool addToScore)
    {
        int scoreValue = 1;
        float totalCategoryMultiplierBonuses = 0;

        // Checking here because the food scriptable objects have not been set up yet
        if(food.foodScriptableObject != null)
        {
            scoreValue = food.foodScriptableObject.pointValue;

            foreach(sFood.FoodCategory category in food.foodScriptableObject.foodCategories)
            {
                totalCategoryMultiplierBonuses += _foodCategoryMultiplierBonus[category];
            }
        }
        else
        {
            Debug.Log("ScoreSystem::AdjustScore - Food Object does not have food scriptable object set: " + food.gameObject.name);
        }

        if(!addToScore)
        {
            scoreValue *= -1;
        }

        scoreValue += (int)(scoreValue * Math.Max(totalCategoryMultiplierBonuses - 1, 0));
        scoreValue += (int)(scoreValue * Math.Max(_globalScoreMultiplierBonus - 1, 0));

        _currentScore += scoreValue;

        // Debugging purposes. Remove when UI is added.
        Debug.Log("ScoreSystem::AdjustScore - Player Score is now: " + _currentScore);
        UpdateTotalScoreDisplay();
    }

    public int GetScore()
    {
        return _currentScore;
    }

    // multiplier is the value by which to multiply the score
    // duration is the number of seconds to set the multiplier for
    public void SetGlobalMultiplier(float multiplier, float duration)
    {
        if(multiplier == 0)
        {
            Debug.LogError("ScoreSystem::SetGlobalMultiplier - multiplier should not be 0!");
            return;
        }

        if(duration <= 0)
        {
            Debug.LogError("ScoreSystem::SetGlobalMultiplier - duration should be greater than 0!");
            return;
        }

        StartCoroutine(TemporaryGlobalScoreMultiplier(multiplier, duration));
    }

    private IEnumerator TemporaryGlobalScoreMultiplier(float multiplier, float duration)
    {
        _globalScoreMultiplierBonus += multiplier;
        yield return new WaitForSeconds(duration);
        _globalScoreMultiplierBonus -= multiplier;

        yield return null;
    }

    // multiplier is the value by which to multiply the score
    // duration is the number of seconds to set the multiplier for
    public void SetCategoryMultiplier(float multiplier, float duration, sFood.FoodCategory foodCategory)
    {
        if(multiplier == 0)
        {
            Debug.LogError("ScoreSystem::SetCategoryMultiplier - multiplier should not be 0!");
            return;
        }

        if(duration <= 0)
        {
            Debug.LogError("ScoreSystem::SetCategoryMultiplier - duration should be greater than 0!");
            return;
        }

        StartCoroutine(TemporaryCategoryScoreMultiplier(multiplier, duration, foodCategory));
    }

    private IEnumerator TemporaryCategoryScoreMultiplier(float multiplier, float duration, sFood.FoodCategory foodCategory)
    {
        _foodCategoryMultiplierBonus[foodCategory] += multiplier;
        yield return new WaitForSeconds(duration);
        _foodCategoryMultiplierBonus[foodCategory] -= multiplier;

        yield return null;
    }

    private void UpdateTotalScoreDisplay()
    {
        if(_totalScoreDisplay != null)
        {
            Transform canvasTransform = _totalScoreDisplay.transform.Find("Canvas");
            Transform totalScoreTransform = canvasTransform.Find("Total Score");
            /*
            Debug.Log("ScoreSystem::UpdateTotalScoreDisplay - totalScoreTransform = " + totalScoreTransform);
            Debug.Log("ScoreSystem::UpdateTotalScoreDisplay - totalScoreTransform.gameObject = " + totalScoreTransform.gameObject);
            Debug.Log("ScoreSystem::UpdateTotalScoreDisplay - totalScoreTransform.gameObject.GetComponent<TextMeshProUGUI>() = " + totalScoreTransform.gameObject.GetComponent<TextMeshProUGUI>());
            */

            TextMeshProUGUI totalScoreTextMp = totalScoreTransform.gameObject.GetComponent<TextMeshProUGUI>();
            totalScoreTextMp.SetText("" + _currentScore);
        }
        else 
        {
            Debug.LogError("ScoreSystem::UpdateTotalScoreDisplay - Score display is not assigned to the Score System");
        }
    }
}
