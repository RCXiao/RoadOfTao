using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CardLayoutManager : MonoBehaviour
{
    public float maxWidth = 7f;
    public float cardSpacing = 2f;


    public Vector3 centerPoint;

    [SerializeField] private List<Vector3> cardPositions = new();
    private List<Quaternion> cardRotations = new();

    private void Awake()
    {
        centerPoint = Vector3.up * -3.6f;
    }

    public CardTransform GetCardTransform(int index, int totalCards)
    {
        CalculatePosition(totalCards);

        return new CardTransform(cardPositions[index], cardRotations[index]);
    }


    private void CalculatePosition(int numberOfCards)
    {
        cardPositions.Clear();
        cardRotations.Clear();


        float currentWidth = cardSpacing * (numberOfCards - 1);
        float totalWidth = Mathf.Min(currentWidth, maxWidth);

        float currentSpacing = totalWidth > 0 ? totalWidth / (numberOfCards - 1) : 0;

        for (int i = 0; i < numberOfCards; i++)
        {
            float xPos = -(float)0.5 - (totalWidth / 2) + (i * currentSpacing);

            var pos = new Vector3(xPos, centerPoint.y, 0f);
            var rot = Quaternion.identity;
            cardPositions.Add(pos);
            cardRotations.Add(rot);
        }
    }
}
