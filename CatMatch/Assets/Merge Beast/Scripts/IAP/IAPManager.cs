using System;
using System.Collections.Generic;
using UnityEngine;
using MergeBeast;
using Observer;

// Placing the Purchaser class in the CompleteProject namespace allows it to interact with ScoreManager, 
// one of the existing Survival Shooter scripts.
// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class IAPManager : MonoBehaviour//, IStoreListener
{
    //private static IStoreController m_StoreController;          // The Unity Purchasing system.
    //private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    [SerializeField] private IAPConfig _config;
    //private  string PRODUCT_NO_ADS =    "mp_removeads";
    //private  string PRODUCT_GEMS1 =    "gems1";
    //private  string PRODUCT_GEMS2 =    "gems2";
    //private  string PRODUCT_GEMS3 =    "gems3";

    public static IAPManager Instance { get; set; }
    public IAPListener IapListener { get; set; }



    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        // If we haven't set up the Unity Purchasing reference
        /*if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }*/
    }


    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }
/*
        Debug.Log($"Init Purchase");
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        for (int i = 0; i < _config.Packs.Count; i++)
        {
            builder.AddProduct(_config.Packs[i].PurchaseID, ProductType.Consumable);
        }
        builder.AddProduct(StringDefine.MONTH_CARD, ProductType.Consumable);
        builder.AddProduct(StringDefine.WEEKLY_1, ProductType.Consumable);
        builder.AddProduct(StringDefine.WEEKLY_2, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
		*/
    }


    private bool IsInitialized()
    {
		return false;
        // Only say we are initialized if both the Purchasing references are set.
        //return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    public void BuyPackage(string id)
    {
        BuyProductID(id);
    }


    private void BuyProductID(string productId)
    {
		/*
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }*/
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }
/*
        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }*/
    }


    //  
    // --- IStoreListener
    //

   /* public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;


#if SUBSCRIPTION_MANAGER
        Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();
#endif
        // This extension function returns a dictionary of the products' skuDetails from GooglePlay Store
        // Key is product Id (Sku), value is the skuDetails json string
        //Dictionary<string, string> google_play_store_product_SKUDetails_json = m_GooglePlayStoreExtensions.GetProductJSONDictionary();

    }*/

/*
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        switch (error)
        {
            case InitializationFailureReason.AppNotKnown:
                Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                // Ask the user if billing is disabled in device settings.
                Debug.Log("Billing disabled!");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                // Developer configuration error; check product metadata.
                Debug.Log("No products available for purchase!");
                break;
        }
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log("Purchase OK: " + args.purchasedProduct.definition.id);
        //            Debug.Log("Receipt: " + args.purchasedProduct.receipt);

        this.PostEvent(EventID.OnPurchaseSuccess, args.purchasedProduct.definition.id);
        //for (int i = 0; i < _config.Packs.Count; i++)
        //{
        //    if (String.Equals(args.purchasedProduct.definition.id, _config.Packs[i].PurchaseID, StringComparison.Ordinal))
        //    {
        //        //Utils.AddRubyCoin((int)_config.Packs[i].GemReceived);
        //        //this.PostEvent(EventID.OnUpDateMoney);
        //        this.PostEvent(EventID.OnPurchaseSuccess, args.purchasedProduct.definition.id);
        //        break;
        //    }
        //}

        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        if (failureReason == PurchaseFailureReason.DuplicateTransaction)
        {
            IapListener.OnPurchaseSuccess(product.definition.storeSpecificId);
        }
    }
*/
    public string LocalizePrice(string id)
    {
       // if (m_StoreController == null) InitializePurchasing();
       // return m_StoreController.products.WithID(id).metadata.localizedPriceString;
	   return "";
    }

}
