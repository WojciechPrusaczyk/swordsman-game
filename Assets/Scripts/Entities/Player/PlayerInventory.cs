using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    
    public bool isEquipmentShown = false;
    public bool isPlayerPickingItem = false;
    public bool isInspectingItems = false;
    public int selectedItemIndex = 0;
    public Sprite fieldImage;
    public Sprite selectedFieldImage;
    public Color secondaryEqColor;
    public Color secondaryItemsListColor;

    private GameObject MainUi;
    private GameObject InventoryUi;
    private GameObject fields;
    private GameObject selectedItemDesc;
    private GameObject incomingItemInfo;
    private ItemsHandler itemsHandler;
    private EntityStatus playerStatus;
    private bool isVerticalInputPressed = false;
    private float ItemsListInitialWidth;
    private Texture rawImage1;
    private Texture rawImage2;
    private bool areImagesSwapped = false;
    private Color primaryEqColor;
    private Color primaryItemsListColor;

    public void Start()
    {
        MainUi = GameObject.Find("Main User Interface");
        InventoryUi = GameObject.Find("Equipment Interface");
        selectedItemDesc = InventoryUi.transform.Find("SelectedItemInfo").gameObject;
        incomingItemInfo = InventoryUi.transform.Find("IncomingItemInfo").gameObject;
        fields = InventoryUi.transform.Find("ItemsFields").gameObject;
        itemsHandler = gameObject.GetComponent<ItemsHandler>();
        playerStatus = gameObject.GetComponent<EntityStatus>();
        ItemsListInitialWidth = InventoryUi.transform.Find("ItemsFields").GetComponent<RectTransform>().offsetMin.x;
        
        rawImage1 = InventoryUi.GetComponent<RawImage>().texture;
        rawImage2 = InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().texture;
        primaryEqColor = InventoryUi.GetComponent<RawImage>().color;
        primaryItemsListColor = InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().color;
            
        HideEquipment();
    }
    
    void Update()
    {
        if ( Input.GetKeyDown( InputManager.InventoryMenuKey ))
        {
            if (isEquipmentShown == false)
            {
                ShowEquipment();
            }
            else
            {
                HideEquipment();
            }
        }

        if (isEquipmentShown)
        {
            float verticalInput = Input.GetAxisRaw("Vertical");

            if (verticalInput < 0 && !isVerticalInputPressed)
            {
                selectedItemIndex = (selectedItemIndex == 0) ? 3 : selectedItemIndex - 1;
                //UpdateEquipmentFrames();
                isVerticalInputPressed = true;
            }
            else if (verticalInput > 0 && !isVerticalInputPressed)
            {
                selectedItemIndex = (selectedItemIndex == 3) ? 0 : selectedItemIndex + 1;
                //UpdateEquipmentFrames();
                isVerticalInputPressed = true;
            }

            if (verticalInput == 0)
            {
                isVerticalInputPressed = false;
            }
            
            // Zmiana rodzaju menu eq przeglądanie przedmiotów / wszystko
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            if (horizontalInput > 0)
            {
                isInspectingItems = true;
                
                // zmiana stanu menu z Inventory na InspectingItem
                selectedItemDesc.SetActive(true);
                InventoryUi.transform.Find("MissionInfo").gameObject.SetActive(false);
                InventoryUi.transform.Find("Elemental").gameObject.SetActive(false);
                
                // wydłużenie width pasku itemów
                RectTransform rectTransform = InventoryUi.transform.Find("ItemsFields").GetComponent<RectTransform>();
                Vector2 offsetMin = rectTransform.offsetMin;
                offsetMin.x = ItemsListInitialWidth - 300.0f; // Ustawienie wartości 'left' na 50
                rectTransform.offsetMin = offsetMin;

                // zamiana tła menu z listą itemów
                InventoryUi.GetComponent<RawImage>().texture = rawImage2;
                InventoryUi.GetComponent<RawImage>().color = secondaryEqColor;
                
                InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().texture = rawImage1;
                InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().color = secondaryItemsListColor;
            }
            else if (horizontalInput < 0)
            {
                isInspectingItems = false;
                // zmiana stanu menu z InspectingItem na Inventory
                selectedItemDesc.SetActive(false);
                InventoryUi.transform.Find("MissionInfo").gameObject.SetActive(true);
                InventoryUi.transform.Find("Elemental").gameObject.SetActive(true);
                
                // skrócenie width pasku itemów
                RectTransform rectTransform = InventoryUi.transform.Find("ItemsFields").GetComponent<RectTransform>();
                Vector2 offsetMin = rectTransform.offsetMin;
                offsetMin.x = ItemsListInitialWidth; // Ustawienie wartości 'left' na 50
                rectTransform.offsetMin = offsetMin;
                
                // zamiana tła menu z listą itemów
                InventoryUi.GetComponent<RawImage>().texture = rawImage1;
                InventoryUi.GetComponent<RawImage>().color = primaryEqColor;
                
                InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().texture = rawImage2;
                InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().color = primaryItemsListColor;
            }
        }
    }

    /*
     * Metoda chowająca ekwipunek
     */
    public void ShowEquipment()
    {
        isEquipmentShown = true;
        isInspectingItems = false;
        isPlayerPickingItem = false;
        Time.timeScale = 0;
        selectedItemIndex = 0;
        //UpdateEquipmentFrames();
        MainUi.SetActive(false);
        InventoryUi.SetActive(true);
        selectedItemDesc.SetActive(false);
        incomingItemInfo.SetActive(false);

        UpdateHp();
        UpdateGold();
        UpdateElemental();
        UpdateExperience();
    }
    
    /*
     * Metoda pokazująca ekwipunek
     */
    public void HideEquipment()
    {
        isEquipmentShown = false;
        Time.timeScale = 1;
        MainUi.SetActive(true);
        InventoryUi.SetActive(false);
    }

    public void UpdateHp()
    {
        try
        {
            GameObject healthObject = InventoryUi.transform.Find("Health").gameObject;

            if (healthObject)
            {
                GameObject healthPoints = healthObject.transform.Find("HP").gameObject;
                GameObject maxHealth = healthObject.transform.Find("MaxHP").gameObject;

                if (healthPoints && maxHealth)
                {
                    healthPoints.GetComponent<TextMeshProUGUI>().text = playerStatus.GetHp().ToString();
                    maxHealth.GetComponent<TextMeshProUGUI>().text = " / " + playerStatus.GetMaxHp().ToString() + "HP";
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    public void UpdateGold()
    {
        try
        {
            TextMeshProUGUI goldCount = InventoryUi.transform.Find("Gold").transform.Find("Count").gameObject.GetComponent<TextMeshProUGUI>();

            if (goldCount)
            {
                goldCount.text = playerStatus.GetGold().ToString() + " g";
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void UpdateExperience()
    {
        try
        {
            GameObject experienceObject = InventoryUi.transform.Find("Experience").gameObject;

            if (experienceObject)
            {
                GameObject level = experienceObject.transform.Find("Level").gameObject;
                GameObject currentXp = experienceObject.transform.Find("Xp").transform.Find("CurrentXp").gameObject;
                GameObject maxXp = experienceObject.transform.Find("Xp").transform.Find("MaxXp").gameObject;

                if (level && currentXp && maxXp)
                {
                    level.GetComponent<TextMeshProUGUI>().text = playerStatus.GetLevel().ToString() + " lvl";
                    currentXp.GetComponent<TextMeshProUGUI>().text = playerStatus.GetXp().ToString();
                    maxXp.GetComponent<TextMeshProUGUI>().text = " / " + playerStatus.GetExpToNextLVl().ToString() + "xp";
                }
                else
                {
                    Debug.Log("Nie znaleziono elementu dziecka w Experience");
                }
            }
            else
            {
                Debug.Log("Nie znaleziono Experience");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void UpdateElemental()
    {
        try
        {
            GameObject elementalParent = InventoryUi.transform.Find("Elemental").gameObject;
            
            // szukanie elementu gracza
            List<Player.ElementalType> elementals = gameObject.GetComponent<Player>().ElementalTypes;
            int UsedElementalTypeId = gameObject.GetComponent<Player>().UsedElementalTypeId;

            Player.ElementalType usedElemental = elementals[UsedElementalTypeId];
            
            if (elementalParent && null != usedElemental)
            {
                GameObject ElementalImage = elementalParent.transform.Find("ElementalImage").gameObject;
                GameObject ElementalName = elementalParent.transform.Find("ElementalName").gameObject;

                if (ElementalImage && ElementalName)
                {
                    ElementalImage.GetComponent<Image>().sprite = usedElemental.icon;
                    ElementalName.GetComponent<TextMeshProUGUI>().text = usedElemental.name;
                    ElementalName.GetComponent<TextMeshProUGUI>().color = usedElemental.elementalColor;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    /*
     * Metoda aktualizująca obrazek w UI, na podstawie podanych danych
     */
    public void SetImageAtSlot(ItemData itemData)
    {
        if (itemData.GetImagePath() != "")
        {
            GameObject selectedSlot = fields.transform.GetChild(selectedItemIndex).Find("ItemImage").gameObject;

            Texture2D texture2D = Resources.Load<Texture2D>(itemData.GetImagePath());
            if (texture2D != null)
            {
                selectedSlot.GetComponent<Image>().sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
                MainUi.transform.Find("Items").GetChild(selectedItemIndex).GetComponent<Image>().sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
                MainUi.transform.Find("Items").GetChild(selectedItemIndex).GetComponent<Image>().color = Color.white;
            }
        }
    }
    
    /*
     * Metoda do dynamicznej aktualizacji obrazka
     */
    public void SetImageAtSlotByIndex(String imagePath, String itemName)
    {
        int itemIndex = itemsHandler.items.FindIndex(obj => obj.GetName() == itemName);
        
        if (imagePath != "")
        {
            GameObject selectedSlot = fields.transform.GetChild(itemIndex).Find("ItemImage").gameObject;

            Texture2D texture2D = Resources.Load<Texture2D>(imagePath);
            if (texture2D != null)
            {
                selectedSlot.GetComponent<Image>().sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
                MainUi.transform.Find("Items").GetChild(itemIndex).GetComponent<Image>().sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
            }
        }
    }

    private void UpdateEquipmentFrames()
    {
        // zwaracnie wszystkich pól eq do podstawowego stanu
        foreach (Transform child in fields.transform)
        {
            // resetowanie obramowania wszystkich pól
            Image image = child.transform.Find("Frame").GetComponent<Image>();
            if (image != null)
            {
                image.sprite = fieldImage;
            }

            // ukrywanie przycisków do zmiany eq
            GameObject actionButtonImage = child.transform.Find("ActionButtonImage").gameObject;
            if (actionButtonImage != null)
            {
                actionButtonImage.SetActive(false);
            }
        }

        // odpowiednie ustawienie pola wybranego
        GameObject selectedField = fields.transform.GetChild(selectedItemIndex).gameObject;
        if (selectedFieldImage != null && selectedField != null)
        {
            // ustawienie obramowania dla wybranego pola
            selectedField.transform.Find("Frame").GetComponent<Image>().sprite = selectedFieldImage;
            
            // pokazywanie przycisku do zmiany ekwipunku
            if (isPlayerPickingItem)
            {
                GameObject actionButtonImage = selectedField.transform.Find("ActionButtonImage").gameObject;
                if (actionButtonImage != null)
                {
                    actionButtonImage.SetActive(true);
                }
            }

            //SetSelectedItemInfo(items[selectedItemIndex]);
        }
    }

    private void SetSelectedItemInfo(ItemData itemData)
    {
        Debug.Log("Ustawienie opisu przedmiotu");
        //Ustawianie opisu przedmiotu
        selectedItemDesc.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = itemData.GetName();
        selectedItemDesc.transform.Find("ActiveDescription").gameObject.GetComponent<TextMeshProUGUI>().text = itemData.GetActiveDescription();
        selectedItemDesc.transform.Find("PassiveDescription").gameObject.GetComponent<TextMeshProUGUI>().text = itemData.GetPassiveDescription();
    }
    private void SetIncomingItemInfo(ItemData itemData)
    {
        //Ustawianie opisu przedmiotu
        incomingItemInfo.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = itemData.GetName();
        incomingItemInfo.transform.Find("ActiveDescription").gameObject.GetComponent<TextMeshProUGUI>().text = itemData.GetActiveDescription();
        incomingItemInfo.transform.Find("PassiveDescription").gameObject.GetComponent<TextMeshProUGUI>().text = itemData.GetPassiveDescription();
    }
}