using UnityEngine;
//using UnityEngine;

//public class ShopController : MonoBehaviour
//{
//    [Header("Shop Tarifleri")]
//    public ShopTradeRecipe[] trades;


//    // ================= TRADE =================
//    public void DoTrade(int index)
//    {
//        if (index < 0 || index >= trades.Length)
//        {
//            return;
//        }         
//        UI_itemController inventory = UI_itemController.instance;
//        if (inventory == null)
//        {
//            return;
//        }          
//        ShopTradeRecipe recipe = trades[index];
//        // 1envanterde yeterli itemler var mı yoksa trade olmaz
//        for (int i = 0; i < recipe.requiredItems.Length; i++)
//        {
//            itemdata GerekliItems = recipe.requiredItems[i];

//            int needed = CountInRecipe(recipe, GerekliItems);
//            int owned = inventory.CountItem(GerekliItems);

//            if (owned < needed)
//            {
//                Debug.Log("❌ TRADE İPTAL → Eksik item: " + GerekliItems.itemName);
//                return;
//            }
//        }
//        //gerekli itemler varsa trade yapılabilir
//        for (int i = 0; i < recipe.requiredItems.Length; i++)
//        {
//            inventory.RemoveAmount(recipe.requiredItems[i], 1);
//        }      
//        // 4️⃣ NORMAL ITEM
//        bool added = inventory.TryAddItem(recipe.rewardItem);

//        if (!added)
//        {
//            Vector3 dropPos =
//                Camera.main.transform.position +
//                Camera.main.transform.forward * 2f;

//            GameObject go =
//                Instantiate(recipe.rewardItem.itemprefab, dropPos, Quaternion.identity);

//            WorldItem wi = go.GetComponent<WorldItem>();
//            if (wi != null)
//                wi.data = recipe.rewardItem;

//            Debug.Log("📦 ENVANTER DOLU → ITEM YERE DÜŞTÜ");
//        }
//        else
//        {
//            Debug.Log("📦 ITEM ENVANTERE EKLENDİ");
//        }       
//    }

//    // TARİFTE AYNI ITEM’TAN KAÇ TANE VAR
//    int CountInRecipe(ShopTradeRecipe recipe, itemdata item)
//    {
//        int count = 0;
//        for (int i = 0; i < recipe.requiredItems.Length; i++)
//        {
//            if (recipe.requiredItems[i].itemID == item.itemID)
//            {
//                count++;
//            }                
//        }
//        return count;
//    }   


//    // trade için ne itemler gerekli ve ne verilecek
//    [System.Serializable]
//    public class ShopTradeRecipe
//    {
//        [Header("Gerekli Itemler")]
//        public itemdata[] requiredItems;

//        [Header("Ödül Item")]
//        public itemdata rewardItem;
//    }
//}

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
                Debug.Log("❌ TRADE İPTAL → Eksik item: " + gerekliItem.itemName);
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

            Debug.Log("📦 ENVANTER DOLU → ITEM YERE DÜŞTÜ");
        }
        else
        {
            Debug.Log("📦 ITEM ENVANTERE EKLENDİ");
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







