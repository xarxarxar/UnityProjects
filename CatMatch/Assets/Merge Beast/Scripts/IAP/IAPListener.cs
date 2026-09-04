using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAPListener
{
	void OnPurchaseSuccess(string productId);

	void OnPurchaseFailue(string productId);
}
