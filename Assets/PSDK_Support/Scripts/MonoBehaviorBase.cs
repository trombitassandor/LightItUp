﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace LightItUp.Game
{
public class MonoBehaviorBase : MonoBehaviour {

	void OnDestroy()
	{
		foreach (FieldInfo field in GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
		{
			Type fieldType = field.FieldType;

			if (typeof(IList).IsAssignableFrom(fieldType))
			{
				IList list = field.GetValue(this) as IList;
				if (list != null)
				{
					list.Clear();
				}
			}

			if (typeof(IDictionary).IsAssignableFrom(fieldType))
			{
				IDictionary dictionary = field.GetValue(this) as IDictionary;
				if (dictionary != null)
				{
					dictionary.Clear();
				}
			}

			if (!fieldType.IsPrimitive)
			{
				field.SetValue(this, null);
			}
		}
	}
}
}
