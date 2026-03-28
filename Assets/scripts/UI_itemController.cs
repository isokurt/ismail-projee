using UnityEngine;
using UnityEngine.UI;

//public class UI_itemController : MonoBehaviour
//{
//    public static UI_itemController instance;

//    [Header("UI")]
//    public Image[] slots;

//    [Header("Slot Renkleri")]
//    public Color normalColor = Color.white;
//    public Color selectedColor = Color.gray;

//    [Header("Slot Ayarları")]
//    public int baseSlotCount = 1;
//    public int backpackSlotCount = 3;

//    [Header("Envanter Verileri")]
//    itemdata[] items = new itemdata[3];
//    int selectedIndex = 0;
//    public  bool isBackpackEquipped = false;

//    void Awake()
//    {
//        if (instance == null) instance = this;
//        else Destroy(gameObject);
//    }

//    void Start()
//    {
//        UpdateSlotAvailability();
//        RefreshUI();
//    }
//    private void Update()
//    {
//        HandleSlotSelection();
//    }

//    // ================= SLOT =================
//    void UpdateSlotAvailability()
//    {
//        // eger backpack takılıysa daha fazla slot aç
//        //degilse sadece base slotu goster
//        int activeSlots = isBackpackEquipped ? backpackSlotCount : baseSlotCount;

//        for (int i = 0; i < slots.Length; i++)
//            slots[i].gameObject.SetActive(i < activeSlots);

//        if (selectedIndex >= activeSlots)
//            selectedIndex = 0;
//    }

//    void RefreshUI()
//    {
//        // UIın hangi slot seciliyse renk degistir ve image guncelle iteme baglı olarak
//        int activeSlots = isBackpackEquipped ? backpackSlotCount : baseSlotCount;

//        for (int i = 0; i < slots.Length; i++)
//        {
//            if (i < activeSlots)
//            {
//                // icon
//                slots[i].sprite = items[i] != null ? items[i].icon : null;

//                // renk
//                slots[i].color = (i == selectedIndex)
//                    ? selectedColor
//                    : normalColor;
//            }
//        }
//    }

//    // ================= ITEM =================
//    public bool TryAddItem(itemdata item)
//    {

//        int activeSlots = isBackpackEquipped ? backpackSlotCount : baseSlotCount;

//        // ÖNCE SEÇİLİ SLOT        
//        if (items[selectedIndex] == null)
//        {
//            items[selectedIndex] = item;
//            RefreshUI();
//            Debug.Log("📦 ITEM SEÇİLİ SLOTA EKLENDI: " + item.itemName);
//            return true;
//        }

//        //SEÇİLİ DOLUYSA DİĞER BOŞLARA BAK
//        for (int i = 0; i < activeSlots; i++)
//        {
//            if (items[i] == null)
//            {
//                items[i] = item;
//                RefreshUI();
//                Debug.Log("📦 ITEM BOŞ SLOTA EKLENDI: " + item.itemName);
//                return true;
//            }
//        }

//        Debug.Log("❌ ENVANTER DOLU");
//        return false;
//    }


//    public bool HasItem(itemdata item)
//    {        
//        foreach (var it in items)
//        {
//            if (it != null && it.itemID == item.itemID)
//            {
//               return true;
//            }                
//        }            
//        return false;
//    }    


//    // ENVANTERDE BELİRLİ ID'DEN KAÇ TANE VAR
//    public int CountItem(itemdata item)
//    {
//        // ID ye gore item say
//        int count = 0;
//        for (int i = 0; i < items.Length; i++)
//        {
//            if (items[i] != null && items[i].itemID == item.itemID)
//            {
//                count++;
//            }               
//        }
//        return count;
//    }

//    //GARANTİLİ SİLME(trade için)
//    public void RemoveAmount(itemdata item, int amount)
//    {
//        int removed = 0;
//        //ID ye gore item sil
//        for (int i = 0; i < items.Length; i++)
//        {            
//            if (items[i] != null && items[i].itemID == item.itemID)
//            {
//                items[i] = null;
//                removed++;

//                if (removed >= amount)
//                    break;
//            }
//        }

//        RefreshUI();
//    }
//    public itemdata GetSelectedItem()
//    {

//        if (selectedIndex < 0 || selectedIndex >= items.Length)
//            return null;

//        return items[selectedIndex];
//    }
//    public void RemoveSelectedItem()
//    {
//        //
//        if (selectedIndex < 0 || selectedIndex >= items.Length)
//            return;

//        if (items[selectedIndex] == null)
//            return;

//        Debug.Log("🗑 SILINDI: " + items[selectedIndex].itemName);
//        items[selectedIndex] = null;

//        RefreshUI();
//    }
//    // ENVANTER DOLUYKEN SEÇİLİ SLOTLA SWAP
//    public void SwapWithSelected(itemdata newItem, Vector3 dropPos)
//    {
//        // seçili itemi al
//        itemdata oldItem = GetSelectedItem();
//        if (oldItem == null) return;

//        // eski itemi yere bırak
//        GameObject go = Instantiate(oldItem.itemprefab, dropPos, Quaternion.identity);
//        WorldItem wi = go.GetComponent<WorldItem>();
//        if (wi != null)
//            wi.data = oldItem;

//        // yeni itemi aynı slota koy
//        items[selectedIndex] = newItem;

//        Debug.Log("🔄 SWAP YAPILDI: " + oldItem.itemName + " → " + newItem.itemName);
//        RefreshUI();
//    }
//    public bool TryEquipSelectedBackpack()
//    {
//        itemdata item = GetSelectedItem();
//        if (item == null) return false;
//        if (!item.isBackpack) return false;
//        if (isBackpackEquipped) return false;

//        // backpack tak
//        isBackpackEquipped = true;

//        // backpack envanterden sil
//        items[selectedIndex] = null;

//        UpdateSlotAvailability();
//        RefreshUI();

//        Debug.Log("🎒 BACKPACK E İLE TAKILDI");
//        return true;
//    }
//    void HandleSlotSelection()
//    {
//        int activeSlots = isBackpackEquipped ? backpackSlotCount : baseSlotCount;

//        if (Input.GetKeyDown(KeyCode.Alpha1) && activeSlots >= 1)
//            SelectSlot(0);

//        if (Input.GetKeyDown(KeyCode.Alpha2) && activeSlots >= 2)
//            SelectSlot(1);

//        if (Input.GetKeyDown(KeyCode.Alpha3) && activeSlots >= 3)
//            SelectSlot(2);
//    }

//    void SelectSlot(int index)
//    {
//        selectedIndex = index;
//        RefreshUI(); // 🔥 BU YOKTU
//        Debug.Log("🟦 SLOT SEÇİLDİ: " + selectedIndex);
//    }


//}

public class UI_itemController : MonoBehaviour
{
    // 🔹 Singleton (her yerden erişebilmek için)
    public static UI_itemController instance;
    
    // ================= UI =================
    [Header("UI")]
    // Envanterdeki slotların Image referansları
    public Image[] slots;

    // ================= SLOT RENKLERİ =================
    [Header("Slot Renkleri")]
    // Normal (seçili olmayan) slot rengi
    public Color normalColor = Color.white;

    // Seçili slot rengi
    public Color selectedColor = Color.gray;

    // ================= SLOT AYARLARI =================
    [Header("Slot Ayarları")]
    // Backpack yokken açık olan slot sayısı
    public int baseSlotCount = 1;

    // Backpack takılıyken açık olan slot sayısı
    public int backpackSlotCount = 3;

    // ================= ENVANTER VERİLERİ =================
    // Gerçek item verilerini tutan array
    itemdata[] items = new itemdata[3];

    // Şu an seçili olan slot indexi
    int selectedIndex = 0;

    // Backpack takılı mı?
    public bool isBackpackEquipped = false;

    // ================= AWAKE =================
    void Awake()
    {
        // Singleton ayarı
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // ================= START =================
    void Start()
    {
        // Hangi slotların açık olacağını ayarla
        UpdateSlotAvailability();

        // UI’ı ilk kez güncelle
        RefreshUI();
    }

    // ================= UPDATE =================
    private void Update()
    {
        // 1-2-3 tuşlarıyla slot seçme
        HandleSlotSelection();
        PlacePlank();
    }

    // ====================================================
    // ================= SLOT SİSTEMİ =====================
    // ====================================================

    void UpdateSlotAvailability()
    {
        // Backpack takılıysa daha fazla slot aç
        // Takılı değilse sadece base slot açık olur
        int activeSlots = isBackpackEquipped ? backpackSlotCount : baseSlotCount;

        // Slotları sırayla aç / kapat
        for (int i = 0; i < slots.Length; i++)
            slots[i].gameObject.SetActive(i < activeSlots);

        // Eğer seçili slot artık yoksa, ilk slota dön
        if (selectedIndex >= activeSlots)
            selectedIndex = 0;
    }

    void RefreshUI()
    {
        // Şu an kaç slot aktif?
        int activeSlots = isBackpackEquipped ? backpackSlotCount : baseSlotCount;

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < activeSlots)
            {
                // Slotta item varsa iconu göster, yoksa boş bırak
                slots[i].sprite = items[i] != null ? items[i].icon : null;

                // Slot seçili mi? Ona göre renk ver
                slots[i].color = (i == selectedIndex)
                    ? selectedColor
                    : normalColor;
            }
        }
    }

    // ====================================================
    // ================= ITEM EKLEME ======================
    // ====================================================

    public bool TryAddItem(itemdata item)
    {
        int activeSlots = isBackpackEquipped ? backpackSlotCount : baseSlotCount;

        // 1️⃣ ÖNCE SEÇİLİ SLOT BOŞ MU?
        if (items[selectedIndex] == null)
        {
            items[selectedIndex] = item;
            RefreshUI();
            Debug.Log("📦 ITEM SEÇİLİ SLOTA EKLENDI: " + item.itemName);
            return true;
        }

        // 2️⃣ SEÇİLİ DOLUYSA DİĞER SLOT’LARA BAK
        for (int i = 0; i < activeSlots; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                RefreshUI();
                Debug.Log("📦 ITEM BOŞ SLOTA EKLENDI: " + item.itemName);
                return true;
            }
        }

        // 3️⃣ HİÇ BOŞ SLOT YOK
        Debug.Log("❌ ENVANTER DOLU");
        return false;
    }

    // ====================================================
    // ================= ITEM KONTROLLERİ =================
    // ====================================================

    // Envanterde bu item var mı?
    public bool HasItem(itemdata item)
    {
        foreach (var it in items)
        {
            if (it != null && it.itemID == item.itemID)
            {
                return true;
            }
        }
        return false;
    }

    // Envanterde bu item’dan kaç tane var?
    public int CountItem(itemdata item)
    {
        int count = 0;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].itemID == item.itemID)
            {
                count++;
            }
        }
        return count;
    }

    // Belirli sayıda item sil (trade için)
    public void RemoveAmount(itemdata item, int amount)
    {
        int removed = 0;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].itemID == item.itemID)
            {
                items[i] = null;
                removed++;

                if (removed >= amount)
                    break;
            }
        }

        RefreshUI();
    }

    // ====================================================
    // ================= SLOT İŞLEMLERİ ===================
    // ====================================================

    // Seçili slottaki item’i al
    public itemdata GetSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Length)
            return null;       

        return items[selectedIndex];
    }
 
    public void PlacePlank()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Length)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (items[selectedIndex] != null && items[selectedIndex].isplank == true)
            {
                UI_PlayerUI1.Instance.SpawnPlank();
                Debug.Log("plank konuldu");
            }
        }            
       
    }

    // Seçili slottaki item’i sil
    public void RemoveSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Length)
            return;

        if (items[selectedIndex] == null)
            return;

        

        Debug.Log("🗑 SILINDI: " + items[selectedIndex].itemName);
        items[selectedIndex] = null;

        RefreshUI();
    }

    // Envanter doluyken seçili slotla item değiştir
    public void SwapWithSelected(itemdata newItem, Vector3 dropPos)
    {
        // Eski item’i al
        itemdata oldItem = GetSelectedItem();
        if (oldItem == null) return;

        // Eski item’i yere bırak
        GameObject go = Instantiate(oldItem.itemprefab, dropPos, Quaternion.identity);
        WorldItem wi = go.GetComponent<WorldItem>();
        if (wi != null)
            wi.data = oldItem;

        // Yeni item’i aynı slota koy
        items[selectedIndex] = newItem;
        

        Debug.Log("🔄 SWAP YAPILDI: " + oldItem.itemName + " → " + newItem.itemName);
        RefreshUI();
    }

    // ====================================================
    // ================= BACKPACK =========================
    // ====================================================

    // Seçili item backpack ise tak
    public bool TryEquipSelectedBackpack()
    {
        itemdata item = GetSelectedItem();
        if (item == null) return false;
        if (!item.isBackpack) return false;
        if (isBackpackEquipped) return false;

        // Backpack takıldı
        isBackpackEquipped = true;

        // Backpack envanterden silinir
        items[selectedIndex] = null;

        UpdateSlotAvailability();
        RefreshUI();

        Debug.Log("🎒 BACKPACK E İLE TAKILDI");
        return true;
    }  


    // ====================================================
    // ================= SLOT SEÇME =======================
    // ====================================================

    void HandleSlotSelection()
    {
        int activeSlots = isBackpackEquipped ? backpackSlotCount : baseSlotCount;

        // Klavyeden 1-2-3 ile slot seç
        if (Input.GetKeyDown(KeyCode.Alpha1) && activeSlots >= 1)
            SelectSlot(0);

        if (Input.GetKeyDown(KeyCode.Alpha2) && activeSlots >= 2)
            SelectSlot(1);

        if (Input.GetKeyDown(KeyCode.Alpha3) && activeSlots >= 3)
            SelectSlot(2);
    }

    void SelectSlot(int index)
    {
        selectedIndex = index;
        RefreshUI();
        Debug.Log("🟦 SLOT SEÇİLDİ: " + selectedIndex);
    }
}

