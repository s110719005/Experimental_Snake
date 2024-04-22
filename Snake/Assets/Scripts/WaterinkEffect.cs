using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterinkEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject waterinkPrefab;
    [SerializeField]
    private List<WaterinkSprites> waterinkSprites;

    [System.Serializable]
    public class WaterinkSprites
    {
        public Sprite[] sprites;
    }
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(WaterinkAnimation(new Vector3(270, -20, 507)));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            PlayWaterinkEffect(new Vector3(270, -20, 507), Color.red);
        }
    }

    public void PlayWaterinkEffect(Vector3 position, Color color)
    {
        StartCoroutine(WaterinkAnimation(position, color));
    }

    private IEnumerator WaterinkAnimation(Vector3 position, Color waterinkColor)
    {
        GameObject waterink = Instantiate(waterinkPrefab, position, waterinkPrefab.transform.rotation);
        SpriteRenderer waterinkSprite = waterink.GetComponent<SpriteRenderer>();
        waterinkSprite.color = waterinkColor;
        int randomIndex = Random.Range(0, waterinkSprites.Count);
        Sprite[] randomSpriteSet = waterinkSprites[randomIndex].sprites;
        for(int i = 0 ; i < randomSpriteSet.Length; i++)
        {
            waterinkSprite.sprite = randomSpriteSet[i];
            Debug.Log("?");
            yield return new WaitForSeconds(0.01f);
        }
        
        waterinkSprite.color = new Color(waterinkSprite.color.r, waterinkSprite.color.g, waterinkSprite.color.b, 1);
        while(waterinkSprite.color.a >= 0.05f)
        {
            waterinkSprite.color = new Color(waterinkSprite.color.r, waterinkSprite.color.g, waterinkSprite.color.b, (waterinkSprite.color.a - 0.005f));
            yield return new WaitForSeconds(0.05f);
        } 
        Destroy(waterink);
    }
}
