using UnityEngine;

public class ShopController : MonoBehaviour
{
    // Shop’ta yapılabilecek takas tarifleri
    [Header("Shop Tarifleri")]
    public ShopTradeRecipe[] trades;
    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // ================= TRADE =================
    public void DoTrade(int index)
    {
        // Geçersiz index kontrolü
        if (index < 0 || index >= trades.Length)
            return;

        // Envantere eriş
        UI_itemController inventory = UI_itemController.instance;
        if (inventory == null)
            return;

        ShopTradeRecipe recipe = trades[index];

        // 1️⃣ GEREKLİ ITEMLER VAR MI?
        for (int i = 0; i < recipe.requiredItems.Length; i++)
        {
            itemdata gerekliItem = recipe.requiredItems[i];

            int needed = CountInRecipe(recipe, gerekliItem);
            int owned = inventory.CountItem(gerekliItem);

            if (owned < needed)
            {
                
                return;
            }
        }

        // 2️⃣ GEREKLİ ITEMLERİ ENVANTERDEN SİL
        for (int i = 0; i < recipe.requiredItems.Length; i++)
        {
            inventory.RemoveAmount(recipe.requiredItems[i], 1);
        }

        // 3️⃣ ÖDÜL ITEM EKLE
        bool added = inventory.TryAddItem(recipe.rewardItem);

        // Envanter doluysa yere düşür
        if (!added)
        {
            Vector3 dropPos =
                Camera.main.transform.position +
                Camera.main.transform.forward * 2f;

            GameObject go =
                Instantiate(recipe.rewardItem.itemprefab, dropPos, Quaternion.identity);

            WorldItem wi = go.GetComponent<WorldItem>();
            if (wi != null)
                wi.data = recipe.rewardItem;

            
        }
        
    }    

    // Tarif içinde aynı item’dan kaç tane var?
    int CountInRecipe(ShopTradeRecipe recipe, itemdata item)
    {
        int count = 0;

        for (int i = 0; i < recipe.requiredItems.Length; i++)
        {
            if (recipe.requiredItems[i].itemID == item.itemID)
                count++;
        }
        return count;
    }

    // ================= TRADE TARİFİ =================
    [System.Serializable]
    public class ShopTradeRecipe
    {
        // Takas için gereken itemler
        [Header("Gerekli Itemler")]
        public itemdata[] requiredItems;

        // Verilecek ödül
        [Header("Ödül Item")]
        public itemdata rewardItem;
    }
}







