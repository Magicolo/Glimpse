using System;
using UnityEngine;

public sealed class PropertyFieldAttribute : CustomAttributeBase {
	
	public Type attributeType;
	public object[] arguments;
	
	public PropertyFieldAttribute(Type attributeType, params object[] arguments) {
		this.attributeType = attributeType;
		this.arguments = arguments;
	}
	
	public PropertyFieldAttribute() {
	}
}
