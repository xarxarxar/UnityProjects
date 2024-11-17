 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SliderTween : MonoBehaviour
{
	public static void ValueTo(GameObject target, Hashtable setValues){ 
		setValues = SliderTween.CleansetValues(setValues);
		
		if (!setValues.Contains("onupdate") || !setValues.Contains("from") || !setValues.Contains("to")) {
			Debug.LogError("  Error: ValueTo() requires an 'onupdate' callback function and a 'from' and 'to' property.  The supplied 'onupdate' callback must accept a single argument that is the same type as the supplied 'from' and 'to' properties!");
			return;
		}else{ 
			setValues["type"]="value";
			
			if (setValues["from"].GetType() == typeof(Vector2)) {
				setValues["method"]="vector2";
			}else if (setValues["from"].GetType() == typeof(Vector3)) {
				setValues["method"]="vector3";
			}else if (setValues["from"].GetType() == typeof(Rect)) {
				setValues["method"]="rect";
			}else if (setValues["from"].GetType() == typeof(Single)) {
				setValues["method"]="float";
			}else{
				return;	
			}
			
 			if (!setValues.Contains("easetype")) {
				setValues.Add("easetype",EaseType.linear);
			}
			
			Launch(target,setValues);
		}
	}
	
 
	
	
	 
	 

	 
	public static void LookFrom(GameObject target, Vector3 looktarget, float time){
		LookFrom(target,Set("looktarget",looktarget,"time",time));
	}	
	 
	public static void LookFrom(GameObject target, Hashtable setValues){
		Vector3 tempRotation;
		Vector3 tempRestriction;
		
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		//set look:
		tempRotation=target.transform.eulerAngles;
		if (setValues["looktarget"].GetType() == typeof(Transform)) {
			//target.transform.LookAt((Transform)setValues["looktarget"]);
			target.transform.LookAt((Transform)setValues["looktarget"], (Vector3?)setValues["up"] ?? Defaults.up);
		}else if(setValues["looktarget"].GetType() == typeof(Vector3)){
			//target.transform.LookAt((Vector3)setValues["looktarget"]);
			target.transform.LookAt((Vector3)setValues["looktarget"], (Vector3?)setValues["up"] ?? Defaults.up);
		}
		
		//axis restriction:
		if(setValues.Contains("axis")){
			tempRestriction=target.transform.eulerAngles;
			switch((string)setValues["axis"]){
				case "x":
				 	tempRestriction.y=tempRotation.y;
					tempRestriction.z=tempRotation.z;
				break;
				case "y":
					tempRestriction.x=tempRotation.x;
					tempRestriction.z=tempRotation.z;
				break;
				case "z":
					tempRestriction.x=tempRotation.x;
					tempRestriction.y=tempRotation.y;
				break;
			}
			target.transform.eulerAngles=tempRestriction;
		}		
		
		//set new rotation:
		setValues["rotation"] = tempRotation;
		
		//establish  
		setValues["type"]="rotate";
		setValues["method"]="to";
		Launch(target,setValues);
	}		
	 
	public static void LookTo(GameObject target, Vector3 looktarget, float time){
		LookTo(target,Set("looktarget",looktarget,"time",time));
	}
	 
	public static void LookTo(GameObject target, Hashtable setValues){		
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);			
		
		//additional property to ensure ConflictCheck can work correctly since Transforms are refrences:		
		if(setValues.Contains("looktarget")){
			if (setValues["looktarget"].GetType() == typeof(Transform)) {
				Transform transform = (Transform)setValues["looktarget"];
				setValues["position"]=new Vector3(transform.position.x,transform.position.y,transform.position.z);
				setValues["rotation"]=new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z);
			}
		}
		
		//establish  
		setValues["type"]="look";
		setValues["method"]="to";
		Launch(target,setValues);
	}		
	 
	public static void MoveTo(GameObject target, Vector3 position, float time){
		MoveTo(target,Set("position",position,"time",time));
	}	
	 
	public static void MoveTo(GameObject target, Hashtable setValues){ 
		setValues = SliderTween.CleansetValues(setValues);
		 	
		if(setValues.Contains("position")){
			if (setValues["position"].GetType() == typeof(Transform)) {
				Transform transform = (Transform)setValues["position"];
				setValues["position"]=new Vector3(transform.position.x,transform.position.y,transform.position.z);
				setValues["rotation"]=new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z);
				setValues["scale"]=new Vector3(transform.localScale.x,transform.localScale.y,transform.localScale.z);
			}
		}		
		 
		setValues["type"]="move";
		setValues["method"]="to";
		Launch(target,setValues);
	}
	 
	public static void MoveFrom(GameObject target, Vector3 position, float time){
		MoveFrom(target,Set("position",position,"time",time));
	}		
	 
	public static void MoveFrom(GameObject target, Hashtable setValues){ 
			setValues = SliderTween.CleansetValues(setValues);
		
		bool tempIsLocal;
		 
		if(setValues.Contains("islocal")){
			tempIsLocal = (bool)setValues["islocal"];
		}else{
			tempIsLocal = Defaults.isLocal;	
		}
		
		if(setValues.Contains("path")){
			Vector3[] fromPath;
			Vector3[] suppliedPath;
			if(setValues["path"].GetType() == typeof(Vector3[])){
				Vector3[] temp = (Vector3[])setValues["path"];
				suppliedPath=new Vector3[temp.Length];
				Array.Copy(temp,suppliedPath, temp.Length);	
			}else{
				Transform[] temp = (Transform[])setValues["path"];
				suppliedPath = new Vector3[temp.Length];
				for (int i = 0; i < temp.Length; i++) {
					suppliedPath[i]=temp[i].position;
				}
			}
			if(suppliedPath[suppliedPath.Length-1] != target.transform.position){
				fromPath= new Vector3[suppliedPath.Length+1];
				Array.Copy(suppliedPath,fromPath,suppliedPath.Length);
				if(tempIsLocal){
					fromPath[fromPath.Length-1] = target.transform.localPosition;
					target.transform.localPosition=fromPath[0];
				}else{
					fromPath[fromPath.Length-1] = target.transform.position;
					target.transform.position=fromPath[0];
				}
				setValues["path"]=fromPath;
			}else{
				if(tempIsLocal){
					target.transform.localPosition=suppliedPath[0];
				}else{
					target.transform.position=suppliedPath[0];
				}
				setValues["path"]=suppliedPath;
			}
		}else{
			Vector3 tempPosition;
			Vector3 fromPosition;
			 
			if(tempIsLocal){
				tempPosition=fromPosition=target.transform.localPosition;
			}else{
				tempPosition=fromPosition=target.transform.position;	
			}
			 
			if(setValues.Contains("position")){
				if (setValues["position"].GetType() == typeof(Transform)){
					Transform trans = (Transform)setValues["position"];
					fromPosition=trans.position;
				}else if(setValues["position"].GetType() == typeof(Vector3)){
					fromPosition=(Vector3)setValues["position"];
				}			
			}else{
				if (setValues.Contains("x")) {
					fromPosition.x=(float)setValues["x"];
				}
				if (setValues.Contains("y")) {
					fromPosition.y=(float)setValues["y"];
				}
				if (setValues.Contains("z")) {
					fromPosition.z=(float)setValues["z"];
				}
			}
			
			//apply fromPosition:
			if(tempIsLocal){
				target.transform.localPosition = fromPosition;
			}else{
				target.transform.position = fromPosition;	
			}
			
			//set new position arg:
			setValues["position"]=tempPosition;
		}
			
		//establish  :
		setValues["type"]="move";
		setValues["method"]="to";
		Launch(target,setValues);
	}
	 
	public static void MoveAdd(GameObject target, Vector3 amount, float time){
		MoveAdd(target,Set("amount",amount,"time",time));
	}
	 
	public static void MoveAdd(GameObject target, Hashtable setValues){
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		//establish  :
		setValues["type"]="move";
		setValues["method"]="add";
		Launch(target,setValues);
	}
	 
	public static void MoveBy(GameObject target, Vector3 amount, float time){
		MoveBy(target,Set("amount",amount,"time",time));
	}
	 
	public static void MoveBy(GameObject target, Hashtable setValues){
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		//establish  :
		setValues["type"]="move";
		setValues["method"]="by";
		Launch(target,setValues);
	}
	 
	public static void ScaleTo(GameObject target, Vector3 scale, float time){
		ScaleTo(target,Set("scale",scale,"time",time));
	}
	 
	public static void ScaleTo(GameObject target, Hashtable setValues){
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		//additional property to ensure ConflictCheck can work correctly since Transforms are refrences:		
		if(setValues.Contains("scale")){
			if (setValues["scale"].GetType() == typeof(Transform)) {
				Transform transform = (Transform)setValues["scale"];
				setValues["position"]=new Vector3(transform.position.x,transform.position.y,transform.position.z);
				setValues["rotation"]=new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z);
				setValues["scale"]=new Vector3(transform.localScale.x,transform.localScale.y,transform.localScale.z);
			}
		}
		
		//establish  :
		setValues["type"]="scale";
		setValues["method"]="to";
		Launch(target,setValues);
	}
	 
	public static void ScaleFrom(GameObject target, Vector3 scale, float time){
		ScaleFrom(target,Set("scale",scale,"time",time));
	}
	 
	public static void ScaleFrom(GameObject target, Hashtable setValues){
		Vector3 tempScale;
		Vector3 fromScale;
	
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		//set base fromScale:
		tempScale=fromScale=target.transform.localScale;
		
		//set augmented fromScale:
		if(setValues.Contains("scale")){
			if (setValues["scale"].GetType() == typeof(Transform)){
				Transform trans = (Transform)setValues["scale"];
				fromScale=trans.localScale;
			}else if(setValues["scale"].GetType() == typeof(Vector3)){
				fromScale=(Vector3)setValues["scale"];
			}	
		}else{
			if (setValues.Contains("x")) {
				fromScale.x=(float)setValues["x"];
			}
			if (setValues.Contains("y")) {
				fromScale.y=(float)setValues["y"];
			}
			if (setValues.Contains("z")) {
				fromScale.z=(float)setValues["z"];
			}
		}
		
		//apply fromScale:
		target.transform.localScale = fromScale;	
		
		//set new scale arg:
		setValues["scale"]=tempScale;
		
		//establish  :
		setValues["type"]="scale";
		setValues["method"]="to";
		Launch(target,setValues);
	}
	
	
	public static void ScaleAdd(GameObject target, Vector3 amount, float time){
		ScaleAdd(target,Set("amount",amount,"time",time));
	}
	
	public static void ScaleAdd(GameObject target, Hashtable setValues){
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		//establish  :
		setValues["type"]="scale";
		setValues["method"]="add";
		Launch(target,setValues);
	}
	 
	public static void ScaleBy(GameObject target, Vector3 amount, float time){
		ScaleBy(target,Set("amount",amount,"time",time));
	}
	
	
	public static void ScaleBy(GameObject target, Hashtable setValues){
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		//establish  :
		setValues["type"]="scale";
		setValues["method"]="by";
		Launch(target,setValues);
	}
	
	public static void RotateTo(GameObject target, Vector3 rotation, float time){
		RotateTo(target,Set("rotation",rotation,"time",time));
	}
	
	public static void RotateTo(GameObject target, Hashtable setValues){
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		//additional property to ensure ConflictCheck can work correctly since Transforms are refrences:		
		if(setValues.Contains("rotation")){
			if (setValues["rotation"].GetType() == typeof(Transform)) {
				Transform transform = (Transform)setValues["rotation"];
				setValues["position"]=new Vector3(transform.position.x,transform.position.y,transform.position.z);
				setValues["rotation"]=new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z);
				setValues["scale"]=new Vector3(transform.localScale.x,transform.localScale.y,transform.localScale.z);
			}
		}		
		
		//establish  
		setValues["type"]="rotate";
		setValues["method"]="to";
		Launch(target,setValues);
	}	

	public static void RotateFrom(GameObject target, Vector3 rotation, float time){
		RotateFrom(target,Set("rotation",rotation,"time",time));
	}
	
	public static void RotateFrom(GameObject target, Hashtable setValues){
		Vector3 tempRotation;
		Vector3 fromRotation;
		bool tempIsLocal;
	
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		//set tempIsLocal:
		if(setValues.Contains("islocal")){
			tempIsLocal = (bool)setValues["islocal"];
		}else{
			tempIsLocal = Defaults.isLocal;	
		}

		//set tempRotation and base fromRotation:
		if(tempIsLocal){
			tempRotation=fromRotation=target.transform.localEulerAngles;
		}else{
			tempRotation=fromRotation=target.transform.eulerAngles;	
		}
		
		//set augmented fromRotation:
		if(setValues.Contains("rotation")){
			if (setValues["rotation"].GetType() == typeof(Transform)){
				Transform trans = (Transform)setValues["rotation"];
				fromRotation=trans.eulerAngles;
			}else if(setValues["rotation"].GetType() == typeof(Vector3)){
				fromRotation=(Vector3)setValues["rotation"];
			}	
		}else{
			if (setValues.Contains("x")) {
				fromRotation.x=(float)setValues["x"];
			}
			if (setValues.Contains("y")) {
				fromRotation.y=(float)setValues["y"];
			}
			if (setValues.Contains("z")) {
				fromRotation.z=(float)setValues["z"];
			}
		}
		
		//apply fromRotation:
		if(tempIsLocal){
			target.transform.localEulerAngles = fromRotation;
		}else{
			target.transform.eulerAngles = fromRotation;	
		}
		
		//set new rotation arg:
		setValues["rotation"]=tempRotation;
		
		//establish  :
		setValues["type"]="rotate";
		setValues["method"]="to";
		Launch(target,setValues);
	}	
	
	public static void RotateAdd(GameObject target, Vector3 amount, float time){
		RotateAdd(target,Set("amount",amount,"time",time));
	}
	
	public static void RotateAdd(GameObject target, Hashtable setValues){
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		//establish  :
		setValues["type"]="rotate";
		setValues["method"]="add";
		Launch(target,setValues);
	}
	
	public static void RotateBy(GameObject target, Vector3 amount, float time){
		RotateBy(target,Set("amount",amount,"time",time));
	}
	
	public static void RotateBy(GameObject target, Hashtable setValues){
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		//establish  
		setValues["type"]="rotate";
		setValues["method"]="by";
		Launch(target,setValues);
	}		
	
	public static void ShakePosition(GameObject target, Vector3 amount, float time){
		ShakePosition(target,Set("amount",amount,"time",time));
	}
	
	
	public static void ShakePosition(GameObject target, Hashtable setValues){
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		//establish  
		setValues["type"]="shake";
		setValues["method"]="position";
		Launch(target,setValues);
	}		
	
	public static void ShakeScale(GameObject target, Vector3 amount, float time){
		ShakeScale(target,Set("amount",amount,"time",time));
	}
	
	
	public static void ShakeScale(GameObject target, Hashtable setValues){
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		//establish  
		setValues["type"]="shake";
		setValues["method"]="scale";
		Launch(target,setValues);
	}		
	
	
	public static void ShakeRotation(GameObject target, Vector3 amount, float time){
		ShakeRotation(target,Set("amount",amount,"time",time));
	}
	
	public static void ShakeRotation(GameObject target, Hashtable setValues){
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		//establish  
		setValues["type"]="shake";
		setValues["method"]="rotation";
		Launch(target,setValues);
	}			
	
	
	public static void PunchPosition(GameObject target, Vector3 amount, float time){
		PunchPosition(target,Set("amount",amount,"time",time));
	}

	public static void PunchPosition(GameObject target, Hashtable setValues){
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		//establish  
		setValues["type"]="punch";
		setValues["method"]="position";
		setValues["easetype"]=EaseType.punch;
		Launch(target,setValues);
	}		
	
	
	public static void PunchRotation(GameObject target, Vector3 amount, float time){
		PunchRotation(target,Set("amount",amount,"time",time));
	}
	
	public static void PunchRotation(GameObject target, Hashtable setValues){
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		//establish  
		setValues["type"]="punch";
		setValues["method"]="rotation";
		setValues["easetype"]=EaseType.punch;
		Launch(target,setValues);
	}	
	
	
	public static void PunchScale(GameObject target, Vector3 amount, float time){
		PunchScale(target,Set("amount",amount,"time",time));
	}
	
	
	public static void PunchScale(GameObject target, Hashtable setValues){
		//clean setValues:
		setValues = SliderTween.CleansetValues(setValues);
		
		setValues["type"]="punch";
		setValues["method"]="scale";
		setValues["easetype"]=EaseType.punch;
		Launch(target,setValues);
	}	
	
	
	void GenerateTargets(){
		switch (type) {
			case "value":
				switch (method) {
					case "float":
						GenerateFloatTargets();
						apply = new ApplyTween(ApplyFloatTargets);
					break;
				case "vector2":
						GenerateVector2Targets();
						apply = new ApplyTween(ApplyVector2Targets);
					break;
				case "vector3":
						GenerateVector3Targets();
						apply = new ApplyTween(ApplyVector3Targets);
					break;
				case "rect":
						GenerateRectTargets();
						apply = new ApplyTween(ApplyRectTargets);
					break;
				}
			break;
			case "move":
				switch (method) {
					case "to":
						//using a path?
						if(tweenArguments.Contains("path")){
							GenerateMoveToPathTargets();
							apply = new ApplyTween(ApplyMoveToPathTargets);
						}else{ //not using a path?
							GenerateMoveToTargets();
							apply = new ApplyTween(ApplyMoveToTargets);
						}
					break;
					case "by":
					case "add":
						GenerateMoveByTargets();
						apply = new ApplyTween(ApplyMoveByTargets);
					break;
				}
			break;
			case "scale":
				switch (method){
					case "to":
						GenerateScaleToTargets();
						apply = new ApplyTween(ApplyScaleToTargets);
					break;
					case "by":
						GenerateScaleByTargets();
						apply = new ApplyTween(ApplyScaleToTargets);
					break;
					case "add":
						GenerateScaleAddTargets();
						apply = new ApplyTween(ApplyScaleToTargets);
					break;
				}
			break;
			case "rotate":
				switch (method) {
					case "to":
						GenerateRotateToTargets();
						apply = new ApplyTween(ApplyRotateToTargets);
					break;
					case "add":
						GenerateRotateAddTargets();
						apply = new ApplyTween(ApplyRotateAddTargets);
					break;
					case "by":
						GenerateRotateByTargets();
						apply = new ApplyTween(ApplyRotateAddTargets);
					break;				
				}
			break;
			case "shake":
				switch (method) {
					case "position":
						GenerateShakePositionTargets();
						apply = new ApplyTween(ApplyShakePositionTargets);
					break;		
					case "scale":
						GenerateShakeScaleTargets();
						apply = new ApplyTween(ApplyShakeScaleTargets);
					break;
					case "rotation":
						GenerateShakeRotationTargets();
						apply = new ApplyTween(ApplyShakeRotationTargets);
					break;
				}
			break;			
			case "punch":
				switch (method) {
					case "position":
						GeneratePunchPositionTargets();
						apply = new ApplyTween(ApplyPunchPositionTargets);
					break;	
					case "rotation":
						GeneratePunchRotationTargets();
						apply = new ApplyTween(ApplyPunchRotationTargets);
					break;	
					case "scale":
						GeneratePunchScaleTargets();
						apply = new ApplyTween(ApplyPunchScaleTargets);
					break;
				}
			break;
			case "look":
				switch (method) {
					case "to":
						GenerateLookToTargets();
						apply = new ApplyTween(ApplyLookToTargets);
					break;	
				}
			break;	
		}
	}
	
	void GenerateRectTargets(){

		rects=new Rect[3];
		
		rects[0]=(Rect)tweenArguments["from"];
		rects[1]=(Rect)tweenArguments["to"];
	}		
	
	void GenerateVector3Targets(){
		vector3s=new Vector3[3];
		
		vector3s[0]=(Vector3)tweenArguments["from"];
		vector3s[1]=(Vector3)tweenArguments["to"];
		
		if(tweenArguments.Contains("speed")){
			float distance = Math.Abs(Vector3.Distance(vector3s[0],vector3s[1]));
			time = distance/(float)tweenArguments["speed"];
		}
	}
	
	void GenerateVector2Targets(){
		vector2s=new Vector2[3];
		
		vector2s[0]=(Vector2)tweenArguments["from"];
		vector2s[1]=(Vector2)tweenArguments["to"];
		if(tweenArguments.Contains("speed")){
			Vector3 fromV3 = new Vector3(vector2s[0].x,vector2s[0].y,0);
			Vector3 toV3 = new Vector3(vector2s[1].x,vector2s[1].y,0);
			float distance = Math.Abs(Vector3.Distance(fromV3,toV3));
			time = distance/(float)tweenArguments["speed"];
		}
	}
	
	void GenerateFloatTargets(){
		floats=new float[3];
		
		//from and to values:
		floats[0]=(float)tweenArguments["from"];
		floats[1]=(float)tweenArguments["to"];
		
		//need for speed?
		if(tweenArguments.Contains("speed")){
			float distance = Math.Abs(floats[0] - floats[1]);
			time = distance/(float)tweenArguments["speed"];
		}
	}
		
	
	void GenerateLookToTargets(){
		//values holder [0] from, [1] to, [2] calculated value from ease equation:
		vector3s=new Vector3[3];
		
		//from values:
		vector3s[0]=thisTransform.eulerAngles;
		
		//set look:
		if(tweenArguments.Contains("looktarget")){
			if (tweenArguments["looktarget"].GetType() == typeof(Transform)) {
				//transform.LookAt((Transform)tweenArguments["looktarget"]);
				thisTransform.LookAt((Transform)tweenArguments["looktarget"], (Vector3?)tweenArguments["up"] ?? Defaults.up);
			}else if(tweenArguments["looktarget"].GetType() == typeof(Vector3)){
				//transform.LookAt((Vector3)tweenArguments["looktarget"]);
				thisTransform.LookAt((Vector3)tweenArguments["looktarget"], (Vector3?)tweenArguments["up"] ?? Defaults.up);
			}
		}else{
			Debug.LogError("  Error: LookTo needs a 'looktarget' property!");
			Dispose();
		}

		//to values:
		vector3s[1]=thisTransform.eulerAngles;
		thisTransform.eulerAngles=vector3s[0];
		
		//axis restriction:
		if(tweenArguments.Contains("axis")){
			switch((string)tweenArguments["axis"]){
				case "x":
					vector3s[1].y=vector3s[0].y;
					vector3s[1].z=vector3s[0].z;
				break;
				case "y":
					vector3s[1].x=vector3s[0].x;
					vector3s[1].z=vector3s[0].z;
				break;
				case "z":
					vector3s[1].x=vector3s[0].x;
					vector3s[1].y=vector3s[0].y;
				break;
			}
		}
		
		//shortest distance:
		vector3s[1]=new Vector3(clerp(vector3s[0].x,vector3s[1].x,1),clerp(vector3s[0].y,vector3s[1].y,1),clerp(vector3s[0].z,vector3s[1].z,1));
		
		//need for speed?
		if(tweenArguments.Contains("speed")){
			float distance = Math.Abs(Vector3.Distance(vector3s[0],vector3s[1]));
			time = distance/(float)tweenArguments["speed"];
		}
	}	
	
	void GenerateMoveToPathTargets(){
		 Vector3[] suppliedPath;
		
		//create and store path points:
		if(tweenArguments["path"].GetType() == typeof(Vector3[])){
			Vector3[] temp = (Vector3[])tweenArguments["path"];
			//if only one point is supplied fall back to MoveTo's traditional use since we can't have a curve with one value:
			if(temp.Length==1){
				Debug.LogError("  Error: Attempting a path movement with MoveTo requires an array of more than 1 entry!");
				Dispose();
			}
			suppliedPath=new Vector3[temp.Length];
			Array.Copy(temp,suppliedPath, temp.Length);
		}else{
			Transform[] temp = (Transform[])tweenArguments["path"];
			//if only one point is supplied fall back to MoveTo's traditional use since we can't have a curve with one value:
			if(temp.Length==1){
				Debug.LogError("  Error: Attempting a path movement with MoveTo requires an array of more than 1 entry!");
				Dispose();
			}
			suppliedPath = new Vector3[temp.Length];
			for (int i = 0; i < temp.Length; i++) {
				suppliedPath[i]=temp[i].position;
			}
		}
		
		//do we need to plot a path to get to the beginning of the supplied path?		
		bool plotStart;
		int offset;
		if(thisTransform.position != suppliedPath[0]){
			if(!tweenArguments.Contains("movetopath") || (bool)tweenArguments["movetopath"]==true){
				plotStart=true;
				offset=3;	
			}else{
				plotStart=false;
				offset=2;
			}
		}else{
			plotStart=false;
			offset=2;
		}				

		//build calculated path:
		vector3s = new Vector3[suppliedPath.Length+offset];
		if(plotStart){
			vector3s[1]=thisTransform.position;
			offset=2;
		}else{
			offset=1;
		}		
		
		//populate calculate path;
		Array.Copy(suppliedPath,0,vector3s,offset,suppliedPath.Length);
		
		//populate start and end control points:
		//vector3s[0] = vector3s[1] - vector3s[2];
		vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
		vector3s[vector3s.Length-1] = vector3s[vector3s.Length-2] + (vector3s[vector3s.Length-2] - vector3s[vector3s.Length-3]);
		
		//is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
		if(vector3s[1] == vector3s[vector3s.Length-2]){
			Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
			Array.Copy(vector3s,tmpLoopSpline,vector3s.Length);
			tmpLoopSpline[0]=tmpLoopSpline[tmpLoopSpline.Length-3];
			tmpLoopSpline[tmpLoopSpline.Length-1]=tmpLoopSpline[2];
			vector3s=new Vector3[tmpLoopSpline.Length];
			Array.Copy(tmpLoopSpline,vector3s,tmpLoopSpline.Length);
		}
		
		//create Catmull-Rom path:
		path = new CRSpline(vector3s);
		
		//need for speed?
		if(tweenArguments.Contains("speed")){
			float distance = PathLength(vector3s);
			time = distance/(float)tweenArguments["speed"];
		}
	}
	
	[HideInInspector]
	public static List<Hashtable> tweens = new List<Hashtable>(); 
	[HideInInspector]
	public string id, type, method;
	[HideInInspector]
	public SliderTween.EaseType easeType;
	[HideInInspector]
	public float time, delay;
	[HideInInspector]
	public LoopType loopType;
	[HideInInspector]
	public bool isRunning,isPaused;
	[HideInInspector]
	public string _name;
 	private float runningTime, percentage;
	private float delayStarted;
	private bool kinematic, isLocal, loop, reverse, wasPaused, physics;
	private Hashtable tweenArguments;
	private Space space;
	private delegate float EasingFunction(float start, float end, float Value);
	private delegate void ApplyTween();
	private EasingFunction ease;
	private ApplyTween apply;
	private Vector3[] vector3s;
	private Vector2[] vector2s;
	private float[] floats;
	private Rect[] rects;
	private CRSpline path;
	[HideInInspector]
	public Vector3 preUpdate;
	private Vector3 postUpdate;
    private float lastRealTime; 
    private bool useRealTime;  
	private Transform thisTransform;

		
	
	public enum EaseType{
		easeInQuad,
		easeOutQuad,
		easeInOutQuad,
		easeInCubic,
		easeOutCubic,
		easeInOutCubic,
		easeInQuart,
		easeOutQuart,
		easeInOutQuart,
		easeInQuint,
		easeOutQuint,
		easeInOutQuint,
		easeInSine,
		easeOutSine,
		easeInOutSine,
		easeInExpo,
		easeOutExpo,
		easeInOutExpo,
		easeInCirc,
		easeOutCirc,
		easeInOutCirc,
		linear,
		spring,
		easeInBounce,
		easeOutBounce,
		easeInOutBounce,
		easeInBack,
		easeOutBack,
		easeInOutBack,
		easeInElastic,
		easeOutElastic,
		easeInOutElastic,
		punch
	}
	

	public enum LoopType{
	
		none,
	
		loop,
	
		pingPong
	}

	
	 
	public static class Defaults{ 
		public static float time = 1f;
		public static float delay = 0f;	
		public static LoopType loopType = LoopType.none;
		public static EaseType easeType = SliderTween.EaseType.easeOutExpo;
		public static float lookSpeed = 3f;
		public static bool isLocal = false;
		public static Space space = Space.Self;
		public static bool orientToPath = false;
		public static float updateTimePercentage = .05f;
		public static float updateTime = 1f*updateTimePercentage; 
		public static float lookAhead = .05f;
        public static bool useRealTime = false;  
		public static Vector3 up = Vector3.up;
	}
	
	
 
	public static void Init(GameObject target){
		MoveBy(target,Vector3.zero,0);
	}
	
	
	void GenerateMoveToTargets(){
		//values holder [0] from, [1] to, [2] calculated value from ease equation:
		vector3s=new Vector3[3];
		
		//from values:
		if (isLocal) {
			vector3s[0]=vector3s[1]=thisTransform.localPosition;				
		}else{
			vector3s[0]=vector3s[1]=thisTransform.position;
		}
		
		//to values:
		if (tweenArguments.Contains("position")) {
			if (tweenArguments["position"].GetType() == typeof(Transform)){
				Transform trans = (Transform)tweenArguments["position"];
				vector3s[1]=trans.position;
			}else if(tweenArguments["position"].GetType() == typeof(Vector3)){
				vector3s[1]=(Vector3)tweenArguments["position"];
			}
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x=(float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y=(float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z=(float)tweenArguments["z"];
			}
		}
		
		//handle orient to path request:
		if(tweenArguments.Contains("orienttopath") && (bool)tweenArguments["orienttopath"]){
			tweenArguments["looktarget"] = vector3s[1];
		}
		
		//need for speed?
		if(tweenArguments.Contains("speed")){
			float distance = Math.Abs(Vector3.Distance(vector3s[0],vector3s[1]));
			time = distance/(float)tweenArguments["speed"];
		}
	}
	
	void GenerateMoveByTargets(){
		//values holder [0] from, [1] to, [2] calculated value from ease equation, [3] previous value for Translate usage to allow Space utilization, [4] original rotation to make sure look requests don't interfere with the direction object should move in, [5] for dial in location:
		vector3s=new Vector3[6];
		
		//grab starting rotation:
		vector3s[4] = thisTransform.eulerAngles;
		
		//from values:
		vector3s[0]=vector3s[1]=vector3s[3]=thisTransform.position;
				
		//to values:
		if (tweenArguments.Contains("amount")) {
			vector3s[1]=vector3s[0] + (Vector3)tweenArguments["amount"];
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x=vector3s[0].x + (float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y=vector3s[0].y +(float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z=vector3s[0].z + (float)tweenArguments["z"];
			}
		}	
		
		//calculation for dial in:
		thisTransform.Translate(vector3s[1],space);
		vector3s[5] = thisTransform.position;
		thisTransform.position=vector3s[0];
		
		//handle orient to path request:
		if(tweenArguments.Contains("orienttopath") && (bool)tweenArguments["orienttopath"]){
			tweenArguments["looktarget"] = vector3s[1];
		}
		
		//need for speed?
		if(tweenArguments.Contains("speed")){
			float distance = Math.Abs(Vector3.Distance(vector3s[0],vector3s[1]));
			time = distance/(float)tweenArguments["speed"];
		}
	}
	
	void GenerateScaleToTargets(){
		//values holder [0] from, [1] to, [2] calculated value from ease equation:
		vector3s=new Vector3[3];
		
		//from values:
		vector3s[0]=vector3s[1]=thisTransform.localScale;				

		//to values:
		if (tweenArguments.Contains("scale")) {
			if (tweenArguments["scale"].GetType() == typeof(Transform)){
				Transform trans = (Transform)tweenArguments["scale"];
				vector3s[1]=trans.localScale;					
			}else if(tweenArguments["scale"].GetType() == typeof(Vector3)){
				vector3s[1]=(Vector3)tweenArguments["scale"];
			}
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x=(float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y=(float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z=(float)tweenArguments["z"];
			}
		} 
		
		//need for speed?
		if(tweenArguments.Contains("speed")){
			float distance = Math.Abs(Vector3.Distance(vector3s[0],vector3s[1]));
			time = distance/(float)tweenArguments["speed"];
		}
	}
	
	void GenerateScaleByTargets(){
		//values holder [0] from, [1] to, [2] calculated value from ease equation:
		vector3s=new Vector3[3];
		
		//from values:
		vector3s[0]=vector3s[1]=thisTransform.localScale;				

		//to values:
		if (tweenArguments.Contains("amount")) {
			vector3s[1]=Vector3.Scale(vector3s[1],(Vector3)tweenArguments["amount"]);
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x*=(float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y*=(float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z*=(float)tweenArguments["z"];
			}
		} 
		
		//need for speed?
		if(tweenArguments.Contains("speed")){
			float distance = Math.Abs(Vector3.Distance(vector3s[0],vector3s[1]));
			time = distance/(float)tweenArguments["speed"];
		}
	}
	
	void GenerateScaleAddTargets(){
		//values holder [0] from, [1] to, [2] calculated value from ease equation:
		vector3s=new Vector3[3];
		
		//from values:
		vector3s[0]=vector3s[1]=thisTransform.localScale;				

		//to values:
		if (tweenArguments.Contains("amount")) {
			vector3s[1]+=(Vector3)tweenArguments["amount"];
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x+=(float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y+=(float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z+=(float)tweenArguments["z"];
			}
		}
		
		//need for speed?
		if(tweenArguments.Contains("speed")){
			float distance = Math.Abs(Vector3.Distance(vector3s[0],vector3s[1]));
			time = distance/(float)tweenArguments["speed"];
		}
	}
	
	void GenerateRotateToTargets(){
		//values holder [0] from, [1] to, [2] calculated value from ease equation:
		vector3s=new Vector3[3];
		
		//from values:
		if (isLocal) {
			vector3s[0]=vector3s[1]=thisTransform.localEulerAngles;				
		}else{
			vector3s[0]=vector3s[1]=thisTransform.eulerAngles;
		}
		
		//to values:
		if (tweenArguments.Contains("rotation")) {
			if (tweenArguments["rotation"].GetType() == typeof(Transform)){
				Transform trans = (Transform)tweenArguments["rotation"];
				vector3s[1]=trans.eulerAngles;			
			}else if(tweenArguments["rotation"].GetType() == typeof(Vector3)){
				vector3s[1]=(Vector3)tweenArguments["rotation"];
			}
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x=(float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y=(float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z=(float)tweenArguments["z"];
			}
		}
		
		//shortest distance:
		vector3s[1]=new Vector3(clerp(vector3s[0].x,vector3s[1].x,1),clerp(vector3s[0].y,vector3s[1].y,1),clerp(vector3s[0].z,vector3s[1].z,1));
		
		//need for speed?
		if(tweenArguments.Contains("speed")){
			float distance = Math.Abs(Vector3.Distance(vector3s[0],vector3s[1]));
			time = distance/(float)tweenArguments["speed"];
		}
	}
	
	void GenerateRotateAddTargets(){
		//values holder [0] from, [1] to, [2] calculated value from ease equation, [3] previous value for Rotate usage to allow Space utilization:
		vector3s=new Vector3[5];
		
		//from values:
		vector3s[0]=vector3s[1]=vector3s[3]=thisTransform.eulerAngles;
		
		//to values:
		if (tweenArguments.Contains("amount")) {
			vector3s[1]+=(Vector3)tweenArguments["amount"];
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x+=(float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y+=(float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z+=(float)tweenArguments["z"];
			}
		}
		
		//need for speed?
		if(tweenArguments.Contains("speed")){
			float distance = Math.Abs(Vector3.Distance(vector3s[0],vector3s[1]));
			time = distance/(float)tweenArguments["speed"];
		}
	}		
	
	void GenerateRotateByTargets(){
		//values holder [0] from, [1] to, [2] calculated value from ease equation, [3] previous value for Rotate usage to allow Space utilization:
		vector3s=new Vector3[4];
		
		//from values:
		vector3s[0]=vector3s[1]=vector3s[3]=thisTransform.eulerAngles;
		
		//to values:
		if (tweenArguments.Contains("amount")) {
			vector3s[1]+=Vector3.Scale((Vector3)tweenArguments["amount"],new Vector3(360,360,360));
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x+=360 * (float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y+=360 * (float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z+=360 * (float)tweenArguments["z"];
			}
		}
		
		//need for speed?
		if(tweenArguments.Contains("speed")){
			float distance = Math.Abs(Vector3.Distance(vector3s[0],vector3s[1]));
			time = distance/(float)tweenArguments["speed"];
		}
	}		
	
	void GenerateShakePositionTargets(){
		//values holder [0] from, [1] to, [2] calculated value from ease equation, [3] original rotation to make sure look requests don't interfere with the direction object should move in:
		vector3s=new Vector3[4];
		
		//grab starting rotation:
		vector3s[3] = thisTransform.eulerAngles;		
		
		//root:
		vector3s[0]=thisTransform.position;
		
		//amount:
		if (tweenArguments.Contains("amount")) {
			vector3s[1]=(Vector3)tweenArguments["amount"];
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x=(float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y=(float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z=(float)tweenArguments["z"];
			}
		}
	}		
	
	void GenerateShakeScaleTargets(){
		//values holder [0] root value, [1] amount, [2] generated amount:
		vector3s=new Vector3[3];
		
		//root:
		vector3s[0]=thisTransform.localScale;
		
		//amount:
		if (tweenArguments.Contains("amount")) {
			vector3s[1]=(Vector3)tweenArguments["amount"];
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x=(float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y=(float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z=(float)tweenArguments["z"];
			}
		}
	}		
		
	void GenerateShakeRotationTargets(){
		//values holder [0] root value, [1] amount, [2] generated amount:
		vector3s=new Vector3[3];
		
		//root:
		vector3s[0]=thisTransform.eulerAngles;
		
		//amount:
		if (tweenArguments.Contains("amount")) {
			vector3s[1]=(Vector3)tweenArguments["amount"];
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x=(float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y=(float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z=(float)tweenArguments["z"];
			}
		}
	}	
	
	void GeneratePunchPositionTargets(){
		//values holder [0] from, [1] to, [2] calculated value from ease equation, [3] previous value for Translate usage to allow Space utilization, [4] original rotation to make sure look requests don't interfere with the direction object should move in:
		vector3s=new Vector3[5];
		
		//grab starting rotation:
		vector3s[4] = thisTransform.eulerAngles;
		
		//from values:
		vector3s[0]=thisTransform.position;
		vector3s[1]=vector3s[3]=Vector3.zero;
				
		//to values:
		if (tweenArguments.Contains("amount")) {
			vector3s[1]=(Vector3)tweenArguments["amount"];
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x=(float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y=(float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z=(float)tweenArguments["z"];
			}
		}
	}	
	
	void GeneratePunchRotationTargets(){
		//values holder [0] from, [1] to, [2] calculated value from ease equation, [3] previous value for Translate usage to allow Space utilization:
		vector3s=new Vector3[4];
		
		//from values:
		vector3s[0]=thisTransform.eulerAngles;
		vector3s[1]=vector3s[3]=Vector3.zero;
				
		//to values:
		if (tweenArguments.Contains("amount")) {
			vector3s[1]=(Vector3)tweenArguments["amount"];
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x=(float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y=(float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z=(float)tweenArguments["z"];
			}
		}
	}		
	
	void GeneratePunchScaleTargets(){
		//values holder [0] from, [1] to, [2] calculated value from ease equation:
		vector3s=new Vector3[3];
		
		//from values:
		vector3s[0]=thisTransform.localScale;
		vector3s[1]=Vector3.zero;
				
		//to values:
		if (tweenArguments.Contains("amount")) {
			vector3s[1]=(Vector3)tweenArguments["amount"];
		}else{
			if (tweenArguments.Contains("x")) {
				vector3s[1].x=(float)tweenArguments["x"];
			}
			if (tweenArguments.Contains("y")) {
				vector3s[1].y=(float)tweenArguments["y"];
			}
			if (tweenArguments.Contains("z")) {
				vector3s[1].z=(float)tweenArguments["z"];
			}
		}
	}
	
	void ApplyRectTargets(){
		//calculate:
		rects[2].x = ease(rects[0].x,rects[1].x,percentage);
		rects[2].y = ease(rects[0].y,rects[1].y,percentage);
		rects[2].width = ease(rects[0].width,rects[1].width,percentage);
		rects[2].height = ease(rects[0].height,rects[1].height,percentage);
		
		//apply:
		tweenArguments["onupdateparams"]=rects[2];
		
		//dial in:
		if(percentage==1){
			tweenArguments["onupdateparams"]=rects[1];
		}
	}		
	
		
	void ApplyVector3Targets(){
		//calculate:
		vector3s[2].x = ease(vector3s[0].x,vector3s[1].x,percentage);
		vector3s[2].y = ease(vector3s[0].y,vector3s[1].y,percentage);
		vector3s[2].z = ease(vector3s[0].z,vector3s[1].z,percentage);
		
		//apply:
		tweenArguments["onupdateparams"]=vector3s[2];
		
		//dial in:
		if(percentage==1){
			tweenArguments["onupdateparams"]=vector3s[1];
		}
	}		
	
	void ApplyVector2Targets(){
		//calculate:
		vector2s[2].x = ease(vector2s[0].x,vector2s[1].x,percentage);
		vector2s[2].y = ease(vector2s[0].y,vector2s[1].y,percentage);
		
		//apply:
		tweenArguments["onupdateparams"]=vector2s[2];
		
		//dial in:
		if(percentage==1){
			tweenArguments["onupdateparams"]=vector2s[1];
		}
	}	
	
	void ApplyFloatTargets(){
		//calculate:
		floats[2] = ease(floats[0],floats[1],percentage);
		
		//apply:
		tweenArguments["onupdateparams"]=floats[2];
		
		//dial in:
		if(percentage==1){
			tweenArguments["onupdateparams"]=floats[1];
		}
	}	
	void ApplyMoveToPathTargets(){
		preUpdate = thisTransform.position;
		float t = ease(0,1,percentage);
		float lookAheadAmount;
		
		//clamp easing equation results as "back" will fail since overshoots aren't handled in the Catmull-Rom interpolation:
		if(isLocal){
			thisTransform.localPosition=path.Interp(Mathf.Clamp(t,0,1));	
		}else{
			thisTransform.position=path.Interp(Mathf.Clamp(t,0,1));	
		}
		
		//handle orient to path request:
		if(tweenArguments.Contains("orienttopath") && (bool)tweenArguments["orienttopath"]){
			
			//plot a point slightly ahead in the interpolation by pushing the percentage forward using the default lookahead value:
			float tLook;
			if(tweenArguments.Contains("lookahead")){
				lookAheadAmount = (float)tweenArguments["lookahead"];
			}else{
				lookAheadAmount = Defaults.lookAhead;
			}
			//tLook = ease(0,1,percentage+lookAheadAmount);			
			tLook = ease(0,1, Mathf.Min(1f, percentage+lookAheadAmount)); 
			
			//locate new leading point with a clamp as stated above:
			//Vector3 lookDistance = path.Interp(Mathf.Clamp(tLook,0,1)) - transform.position;
			tweenArguments["looktarget"] = path.Interp(Mathf.Clamp(tLook,0,1));
		}
		
		//need physics?
		postUpdate=thisTransform.position;
		if(physics){
			//thisTransform.position=preUpdate;
			GetComponent<Rigidbody>().MovePosition(postUpdate);
		}
	}
	
	void ApplyMoveToTargets(){
		//record current:
		preUpdate=thisTransform.position;
			
		
		//calculate:
		vector3s[2].x = ease(vector3s[0].x,vector3s[1].x,percentage);
		vector3s[2].y = ease(vector3s[0].y,vector3s[1].y,percentage);
		vector3s[2].z = ease(vector3s[0].z,vector3s[1].z,percentage);

 
		//apply:	
		if (isLocal) {
			thisTransform.localPosition=vector3s[2];
		}else{
			thisTransform.position=vector3s[2];
		}
			
		//dial in:
		if(percentage==1){
			if (isLocal) {
				thisTransform.localPosition=vector3s[1];		
			}else{
				thisTransform.position=vector3s[1];
			}
		}
			
		//need physics?
		postUpdate=thisTransform.position;
		if(physics){
			//thisTransform.position=preUpdate;
			GetComponent<Rigidbody>().MovePosition(postUpdate);
		}
	}	
	
	void ApplyMoveByTargets(){	
		preUpdate = thisTransform.position;
		
		//reset rotation to prevent look interferences as object rotates and attempts to move with translate and record current rotation
		Vector3 currentRotation = new Vector3();
		
		if(tweenArguments.Contains("looktarget")){
			currentRotation = thisTransform.eulerAngles;
			thisTransform.eulerAngles = vector3s[4];	
		}
		
		//calculate:
		vector3s[2].x = ease(vector3s[0].x,vector3s[1].x,percentage);
		vector3s[2].y = ease(vector3s[0].y,vector3s[1].y,percentage);
		vector3s[2].z = ease(vector3s[0].z,vector3s[1].z,percentage);
				
		//apply:
		thisTransform.Translate(vector3s[2]-vector3s[3],space);
		
		//record:
		vector3s[3]=vector3s[2];
		
		//reset rotation:
		if(tweenArguments.Contains("looktarget")){
			thisTransform.eulerAngles = currentRotation;	
		}
				
		/*
		//dial in:
		if(percentage==1){	
			transform.position=vector3s[5];
		}
		*/
		
		//need physics?
		postUpdate=thisTransform.position;
		if(physics){
			//thisTransform.position=preUpdate;
			GetComponent<Rigidbody>().MovePosition(postUpdate);
		}
	}	
	
	void ApplyScaleToTargets(){
		//calculate:
		vector3s[2].x = ease(vector3s[0].x,vector3s[1].x,percentage);
		vector3s[2].y = ease(vector3s[0].y,vector3s[1].y,percentage);
		vector3s[2].z = ease(vector3s[0].z,vector3s[1].z,percentage);
		
		//apply:
		thisTransform.localScale=vector3s[2];	
		
		//dial in:
		if(percentage==1){
			thisTransform.localScale=vector3s[1];
		}
	}
	
	void ApplyLookToTargets(){
		//calculate:
		vector3s[2].x = ease(vector3s[0].x,vector3s[1].x,percentage);
		vector3s[2].y = ease(vector3s[0].y,vector3s[1].y,percentage);
		vector3s[2].z = ease(vector3s[0].z,vector3s[1].z,percentage);
		
		//apply:
		if (isLocal) {
			thisTransform.localRotation = Quaternion.Euler(vector3s[2]);
		}else{
			thisTransform.rotation = Quaternion.Euler(vector3s[2]);
		};	
	}	
	
	void ApplyRotateToTargets(){
		preUpdate=thisTransform.eulerAngles;
		
		//calculate:
		vector3s[2].x = ease(vector3s[0].x,vector3s[1].x,percentage);
		vector3s[2].y = ease(vector3s[0].y,vector3s[1].y,percentage);
		vector3s[2].z = ease(vector3s[0].z,vector3s[1].z,percentage);
		
		//apply:
		if (isLocal) {
			thisTransform.localRotation = Quaternion.Euler(vector3s[2]);
		}else{
			thisTransform.rotation = Quaternion.Euler(vector3s[2]);
		};	
		
		//dial in:
		if(percentage==1){
			if (isLocal) {
				thisTransform.localRotation = Quaternion.Euler(vector3s[1]);
			}else{
				thisTransform.rotation = Quaternion.Euler(vector3s[1]);
			};
		}
		
		//need physics?
		postUpdate=thisTransform.eulerAngles;
		if(physics){
			//thisTransform.eulerAngles=preUpdate;
			GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(postUpdate));
		}
	}
	
	void ApplyRotateAddTargets(){
		preUpdate = thisTransform.eulerAngles;
		
		//calculate:
		vector3s[2].x = ease(vector3s[0].x,vector3s[1].x,percentage);
		vector3s[2].y = ease(vector3s[0].y,vector3s[1].y,percentage);
		vector3s[2].z = ease(vector3s[0].z,vector3s[1].z,percentage);
		
		//apply:
		thisTransform.Rotate(vector3s[2]-vector3s[3],space);

		//record:
		vector3s[3]=vector3s[2];	
		
		//need physics?
		postUpdate=thisTransform.eulerAngles;
		if(physics){
			//thisTransform.eulerAngles=preUpdate;
			GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(postUpdate));
		}		
	}	
	
	void ApplyShakePositionTargets(){
		//preUpdate = transform.position;
		if (isLocal) {
			preUpdate = thisTransform.localPosition;
		}else{
			preUpdate = thisTransform.position;
		}
		
		//reset rotation to prevent look interferences as object rotates and attempts to move with translate and record current rotation
		Vector3 currentRotation = new Vector3();
		
		if(tweenArguments.Contains("looktarget")){
			currentRotation = thisTransform.eulerAngles;
			thisTransform.eulerAngles = vector3s[3];	
		}
		
		//impact:
		if (percentage==0) {
			thisTransform.Translate(vector3s[1],space);
		}
		
		//transform.position=vector3s[0];
		//reset:
		if (isLocal) {
			thisTransform.localPosition=vector3s[0];
		}else{
			thisTransform.position=vector3s[0];
		}
		
		//generate:
		float diminishingControl = 1-percentage;
		vector3s[2].x= UnityEngine.Random.Range(-vector3s[1].x*diminishingControl, vector3s[1].x*diminishingControl);
		vector3s[2].y= UnityEngine.Random.Range(-vector3s[1].y*diminishingControl, vector3s[1].y*diminishingControl);
		vector3s[2].z= UnityEngine.Random.Range(-vector3s[1].z*diminishingControl, vector3s[1].z*diminishingControl);

		//apply:	
		//transform.Translate(vector3s[2],space);	
		if (isLocal) {
			thisTransform.localPosition+=vector3s[2];
		}else{
			thisTransform.position+=vector3s[2];
		}
		
		//reset rotation:
		if(tweenArguments.Contains("looktarget")){
			thisTransform.eulerAngles = currentRotation;	
		}	
		
		//need physics?
		postUpdate=thisTransform.position;
		if(physics){
			//thisTransform.position=preUpdate;
			GetComponent<Rigidbody>().MovePosition(postUpdate);
		}
	}	
	
	void ApplyShakeScaleTargets(){
		//impact:
		if (percentage==0) {
			thisTransform.localScale=vector3s[1];
		}
		
		//reset:
		thisTransform.localScale=vector3s[0];
		
		//generate:
		float diminishingControl = 1-percentage;
		vector3s[2].x= UnityEngine.Random.Range(-vector3s[1].x*diminishingControl, vector3s[1].x*diminishingControl);
		vector3s[2].y= UnityEngine.Random.Range(-vector3s[1].y*diminishingControl, vector3s[1].y*diminishingControl);
		vector3s[2].z= UnityEngine.Random.Range(-vector3s[1].z*diminishingControl, vector3s[1].z*diminishingControl);

		//apply:
		thisTransform.localScale+=vector3s[2];
	}		
	
	void ApplyShakeRotationTargets(){
		preUpdate = thisTransform.eulerAngles;
		
		//impact:
		if (percentage==0) {
			thisTransform.Rotate(vector3s[1],space);
		}
		
		//reset:
		thisTransform.eulerAngles=vector3s[0];
		
		//generate:
		float diminishingControl = 1-percentage;
		vector3s[2].x= UnityEngine.Random.Range(-vector3s[1].x*diminishingControl, vector3s[1].x*diminishingControl);
		vector3s[2].y= UnityEngine.Random.Range(-vector3s[1].y*diminishingControl, vector3s[1].y*diminishingControl);
		vector3s[2].z= UnityEngine.Random.Range(-vector3s[1].z*diminishingControl, vector3s[1].z*diminishingControl);

		//apply:
		thisTransform.Rotate(vector3s[2],space);
		
		//need physics?
		postUpdate=thisTransform.eulerAngles;
		if(physics){
			//thisTransform.eulerAngles=preUpdate;
			GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(postUpdate));
		}
	}		
	
	void ApplyPunchPositionTargets(){
		preUpdate = thisTransform.position;
		
		//reset rotation to prevent look interferences as object rotates and attempts to move with translate and record current rotation
		Vector3 currentRotation = new Vector3();
		
		if(tweenArguments.Contains("looktarget")){
			currentRotation = thisTransform.eulerAngles;
			thisTransform.eulerAngles = vector3s[4];	
		}
		
		//calculate:
		if(vector3s[1].x>0){
			vector3s[2].x = punch(vector3s[1].x,percentage);
		}else if(vector3s[1].x<0){
			vector3s[2].x=-punch(Mathf.Abs(vector3s[1].x),percentage); 
		}
		if(vector3s[1].y>0){
			vector3s[2].y=punch(vector3s[1].y,percentage);
		}else if(vector3s[1].y<0){
			vector3s[2].y=-punch(Mathf.Abs(vector3s[1].y),percentage); 
		}
		if(vector3s[1].z>0){
			vector3s[2].z=punch(vector3s[1].z,percentage);
		}else if(vector3s[1].z<0){
			vector3s[2].z=-punch(Mathf.Abs(vector3s[1].z),percentage); 
		}
		
		//apply:
		thisTransform.Translate(vector3s[2]-vector3s[3],space);

		//record:
		vector3s[3]=vector3s[2];
		
		//reset rotation:
		if(tweenArguments.Contains("looktarget")){
			thisTransform.eulerAngles = currentRotation;	
		}
		
		//dial in:
		/*
		if(percentage==1){	
			transform.position=vector3s[0];
		}
		*/
		
		//need physics?
		postUpdate=thisTransform.position;
		if(physics){
			//thisTransform.position=preUpdate;
			GetComponent<Rigidbody>().MovePosition(postUpdate);
		}
	}		
	
	void ApplyPunchRotationTargets(){
		preUpdate = thisTransform.eulerAngles;
		
		//calculate:
		if(vector3s[1].x>0){
			vector3s[2].x = punch(vector3s[1].x,percentage);
		}else if(vector3s[1].x<0){
			vector3s[2].x=-punch(Mathf.Abs(vector3s[1].x),percentage); 
		}
		if(vector3s[1].y>0){
			vector3s[2].y=punch(vector3s[1].y,percentage);
		}else if(vector3s[1].y<0){
			vector3s[2].y=-punch(Mathf.Abs(vector3s[1].y),percentage); 
		}
		if(vector3s[1].z>0){
			vector3s[2].z=punch(vector3s[1].z,percentage);
		}else if(vector3s[1].z<0){
			vector3s[2].z=-punch(Mathf.Abs(vector3s[1].z),percentage); 
		}
		
		//apply:
		thisTransform.Rotate(vector3s[2]-vector3s[3],space);

		//record:
		vector3s[3]=vector3s[2];
		
		//dial in:
		/*
		if(percentage==1){	
			transform.eulerAngles=vector3s[0];
		}
		*/
		
		//need physics?
		postUpdate=thisTransform.eulerAngles;
		if(physics){
			//thisTransform.eulerAngles=preUpdate;
			GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(postUpdate));
		}
	}	
	
	void ApplyPunchScaleTargets(){
		//calculate:
		if(vector3s[1].x>0){
			vector3s[2].x = punch(vector3s[1].x,percentage);
		}else if(vector3s[1].x<0){
			vector3s[2].x=-punch(Mathf.Abs(vector3s[1].x),percentage); 
		}
		if(vector3s[1].y>0){
			vector3s[2].y=punch(vector3s[1].y,percentage);
		}else if(vector3s[1].y<0){
			vector3s[2].y=-punch(Mathf.Abs(vector3s[1].y),percentage); 
		}
		if(vector3s[1].z>0){
			vector3s[2].z=punch(vector3s[1].z,percentage);
		}else if(vector3s[1].z<0){
			vector3s[2].z=-punch(Mathf.Abs(vector3s[1].z),percentage); 
		}
		
		thisTransform.localScale=vector3s[0]+vector3s[2];
		
	}		
	
	IEnumerator TweenDelay(){
		delayStarted = Time.time;
		yield return new WaitForSeconds (delay);
		if(wasPaused){
			wasPaused=false;
			TweenStart();	
		}
	}	
	
	void TweenStart(){		
		CallBack("onstart");
		
		if(!loop){//only if this is not a loop
			ConflictCheck();
			GenerateTargets();
		}
		
		
		//toggle isKinematic for  s that may interfere with physics:
		if (type == "move" || type=="scale" || type=="rotate" || type=="punch" || type=="shake" || type=="curve" || type=="look") {
			EnableKinematic();
		}
		
		isRunning = true;
	}
	
	IEnumerator TweenRestart(){
		if(delay > 0){
			delayStarted = Time.time;
			yield return new WaitForSeconds (delay);
		}
		loop=true;
		TweenStart();
	}	
	
	void TweenUpdate(){
		apply();
		CallBack("onupdate");
		UpdatePercentage();		
	}
			
	void TweenComplete(){
		isRunning=false;
		
		//dial in percentage to 1 or 0 for final run:
		if(percentage>.5f){
			percentage=1f;
		}else{
			percentage=0;	
		}
		
		//apply dial in and final run:
		apply();
		if(type == "value"){
			CallBack("onupdate"); //CallBack run for ValueTo since it only calculates and applies in the update callback
		}
		
		//loop or dispose?
		if(loopType==LoopType.none){
			Dispose();
		}else{
			TweenLoop();
		}
		
		CallBack("oncomplete");
	}
	
	void TweenLoop(){
		DisableKinematic(); //give physics control again
		switch(loopType){
			case LoopType.loop:
				//rewind:
				percentage=0;
				runningTime=0;
				apply();
				
				//replay:
				StartCoroutine("TweenRestart");
				break;
			case LoopType.pingPong:
				reverse = !reverse;
				runningTime=0;
			
				StartCoroutine("TweenRestart");
				break;
		}
	}	
	
	public static Rect RectUpdate(Rect currentValue, Rect targetValue, float speed){
		Rect diff = new Rect(FloatUpdate(currentValue.x, targetValue.x, speed), FloatUpdate(currentValue.y, targetValue.y, speed), FloatUpdate(currentValue.width, targetValue.width, speed), FloatUpdate(currentValue.height, targetValue.height, speed));
		return (diff);
	}
	
	
	public static Vector3 Vector3Update(Vector3 currentValue, Vector3 targetValue, float speed){
		Vector3 diff = targetValue - currentValue;
		currentValue += (diff * speed) * Time.deltaTime;
		return (currentValue);
	}
	
	
	public static Vector2 Vector2Update(Vector2 currentValue, Vector2 targetValue, float speed){
		Vector2 diff = targetValue - currentValue;
		currentValue += (diff * speed) * Time.deltaTime;
		return (currentValue);
	}
	
	
	public static float FloatUpdate(float currentValue, float targetValue, float speed){
		float diff = targetValue - currentValue;
		currentValue += (diff * speed) * Time.deltaTime;
		return (currentValue);
	}
	
	


	

	
	public static void RotateUpdate(GameObject target, Hashtable setValues){
		CleansetValues(setValues);
		
		bool isLocal;
		float time;
		Vector3[] vector3s = new Vector3[4];
		Vector3 preUpdate = target.transform.eulerAngles;
		
		//set smooth time:
		if(setValues.Contains("time")){
			time=(float)setValues["time"];
			time*=Defaults.updateTimePercentage;
		}else{
			time=Defaults.updateTime;
		}
		
		//set isLocal:
		if(setValues.Contains("islocal")){
			isLocal = (bool)setValues["islocal"];
		}else{
			isLocal = Defaults.isLocal;	
		}
		
		//from values:
		if(isLocal){
			vector3s[0] = target.transform.localEulerAngles;
		}else{
			vector3s[0] = target.transform.eulerAngles;	
		}
		
		//set to:
		if(setValues.Contains("rotation")){
			if (setValues["rotation"].GetType() == typeof(Transform)){
				Transform trans = (Transform)setValues["rotation"];
				vector3s[1]=trans.eulerAngles;
			}else if(setValues["rotation"].GetType() == typeof(Vector3)){
				vector3s[1]=(Vector3)setValues["rotation"];
			}	
		}
				
		//calculate:
		vector3s[3].x=Mathf.SmoothDampAngle(vector3s[0].x,vector3s[1].x,ref vector3s[2].x,time);
		vector3s[3].y=Mathf.SmoothDampAngle(vector3s[0].y,vector3s[1].y,ref vector3s[2].y,time);
		vector3s[3].z=Mathf.SmoothDampAngle(vector3s[0].z,vector3s[1].z,ref vector3s[2].z,time);
	
		//apply:
		if(isLocal){
			target.transform.localEulerAngles=vector3s[3];
		}else{
			target.transform.eulerAngles=vector3s[3];
		}
		
		//need physics?
		if(target.GetComponent<Rigidbody>() != null){
			Vector3 postUpdate=target.transform.eulerAngles;
			//target.transform.eulerAngles=preUpdate;
			target.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(postUpdate));
		}
	}
		

	public static void RotateUpdate(GameObject target, Vector3 rotation, float time){
		RotateUpdate(target,Set("rotation",rotation,"time",time));
	}
	
	 
	public static void ScaleUpdate(GameObject target, Hashtable setValues){
		CleansetValues(setValues);
		
		float time;
		Vector3[] vector3s = new Vector3[4];
			
		//set smooth time:
		if(setValues.Contains("time")){
			time=(float)setValues["time"];
			time*=Defaults.updateTimePercentage;
		}else{
			time=Defaults.updateTime;
		}
		
		//init values:
		vector3s[0] = vector3s[1] = target.transform.localScale;
		
		//to values:
		if (setValues.Contains("scale")) {
			if (setValues["scale"].GetType() == typeof(Transform)){
				Transform trans = (Transform)setValues["scale"];
				vector3s[1]=trans.localScale;
			}else if(setValues["scale"].GetType() == typeof(Vector3)){
				vector3s[1]=(Vector3)setValues["scale"];
			}				
		}else{
			if (setValues.Contains("x")) {
				vector3s[1].x=(float)setValues["x"];
			}
			if (setValues.Contains("y")) {
				vector3s[1].y=(float)setValues["y"];
			}
			if (setValues.Contains("z")) {
				vector3s[1].z=(float)setValues["z"];
			}
		}
		
		//calculate:
		vector3s[3].x=Mathf.SmoothDamp(vector3s[0].x,vector3s[1].x,ref vector3s[2].x,time);
		vector3s[3].y=Mathf.SmoothDamp(vector3s[0].y,vector3s[1].y,ref vector3s[2].y,time);
		vector3s[3].z=Mathf.SmoothDamp(vector3s[0].z,vector3s[1].z,ref vector3s[2].z,time);
				
		//apply:
		target.transform.localScale=vector3s[3];		
	}	
	

	public static void ScaleUpdate(GameObject target, Vector3 scale, float time){
		ScaleUpdate(target,Set("scale",scale,"time",time));
	}
	
	
	public static void MoveUpdate(GameObject target, Hashtable setValues){
		CleansetValues(setValues);
		
		float time;
		Vector3[] vector3s = new Vector3[4];
		bool isLocal;
		Vector3 preUpdate = target.transform.position;
			
		//set smooth time:
		if(setValues.Contains("time")){
			time=(float)setValues["time"];
			time*=Defaults.updateTimePercentage;
		}else{
			time=Defaults.updateTime;
		}
			
		//set isLocal:
		if(setValues.Contains("islocal")){
			isLocal = (bool)setValues["islocal"];
		}else{
			isLocal = Defaults.isLocal;	
		}
		 
		//init values:
		if(isLocal){
			vector3s[0] = vector3s[1] = target.transform.localPosition;
		}else{
			vector3s[0] = vector3s[1] = target.transform.position;	
		}
		
		//to values:
		if (setValues.Contains("position")) {
			if (setValues["position"].GetType() == typeof(Transform)){
				Transform trans = (Transform)setValues["position"];
				vector3s[1]=trans.position;
			}else if(setValues["position"].GetType() == typeof(Vector3)){
				vector3s[1]=(Vector3)setValues["position"];
			}			
		}else{
			if (setValues.Contains("x")) {
				vector3s[1].x=(float)setValues["x"];
			}
			if (setValues.Contains("y")) {
				vector3s[1].y=(float)setValues["y"];
			}
			if (setValues.Contains("z")) {
				vector3s[1].z=(float)setValues["z"];
			}
		}
		
		//calculate:
		vector3s[3].x=Mathf.SmoothDamp(vector3s[0].x,vector3s[1].x,ref vector3s[2].x,time);
		vector3s[3].y=Mathf.SmoothDamp(vector3s[0].y,vector3s[1].y,ref vector3s[2].y,time);
		vector3s[3].z=Mathf.SmoothDamp(vector3s[0].z,vector3s[1].z,ref vector3s[2].z,time);
			
		//handle orient to path:
		if(setValues.Contains("orienttopath") && (bool)setValues["orienttopath"]){
			setValues["looktarget"] = vector3s[3];
		}
		
		//look applications:
		if(setValues.Contains("looktarget")){
			SliderTween.LookUpdate(target,setValues);
		}
		
		//apply:
		if(isLocal){
			target.transform.localPosition = vector3s[3];			
		}else{
			target.transform.position=vector3s[3];	
		}	
		
		//need physics?
		if(target.GetComponent<Rigidbody>() != null){
			Vector3 postUpdate=target.transform.position;
			//target.transform.position=preUpdate;
			target.GetComponent<Rigidbody>().MovePosition(postUpdate);
		}
	}

	
	public static void MoveUpdate(GameObject target, Vector3 position, float time){
		MoveUpdate(target,Set("position",position,"time",time));
	}
	
	
	public static void LookUpdate(GameObject target, Hashtable setValues){
		CleansetValues(setValues);
		
		float time;
		Vector3[] vector3s = new Vector3[5];
		
		//set smooth time:
		if(setValues.Contains("looktime")){
			time=(float)setValues["looktime"];
			time*=Defaults.updateTimePercentage;
		}else if(setValues.Contains("time")){
			time=(float)setValues["time"]*.15f;
			time*=Defaults.updateTimePercentage;
		}else{
			time=Defaults.updateTime;
		}
		
		//from values:
		vector3s[0] = target.transform.eulerAngles;
		
		//set look:
		if(setValues.Contains("looktarget")){
			if (setValues["looktarget"].GetType() == typeof(Transform)) {
				//target.transform.LookAt((Transform)setValues["looktarget"]);
				target.transform.LookAt((Transform)setValues["looktarget"], (Vector3?)setValues["up"] ?? Defaults.up);
			}else if(setValues["looktarget"].GetType() == typeof(Vector3)){
				//target.transform.LookAt((Vector3)setValues["looktarget"]);
				target.transform.LookAt((Vector3)setValues["looktarget"], (Vector3?)setValues["up"] ?? Defaults.up);
			}
		}else{
			Debug.LogError("  Error: LookUpdate needs a 'looktarget' property!");
			return;
		}
		
		//to values and reset look:
		vector3s[1]=target.transform.eulerAngles;
		target.transform.eulerAngles=vector3s[0];
		
		//calculate:
		vector3s[3].x=Mathf.SmoothDampAngle(vector3s[0].x,vector3s[1].x,ref vector3s[2].x,time);
		vector3s[3].y=Mathf.SmoothDampAngle(vector3s[0].y,vector3s[1].y,ref vector3s[2].y,time);
		vector3s[3].z=Mathf.SmoothDampAngle(vector3s[0].z,vector3s[1].z,ref vector3s[2].z,time);
	
		//apply:
		target.transform.eulerAngles=vector3s[3];
		
		//axis restriction:
		if(setValues.Contains("axis")){
			vector3s[4]=target.transform.eulerAngles;
			switch((string)setValues["axis"]){
				case "x":
					vector3s[4].y=vector3s[0].y;
					vector3s[4].z=vector3s[0].z;
				break;
				case "y":
					vector3s[4].x=vector3s[0].x;
					vector3s[4].z=vector3s[0].z;
				break;
				case "z":
					vector3s[4].x=vector3s[0].x;
					vector3s[4].y=vector3s[0].y;
				break;
			}
			
			//apply axis restriction:
			target.transform.eulerAngles=vector3s[4];
		}	
	}
	
	
	public static void LookUpdate(GameObject target, Vector3 looktarget, float time){
		LookUpdate(target,Set("looktarget",looktarget,"time",time));
	}

	public static float PathLength(Transform[] path){
		Vector3[] suppliedPath = new Vector3[path.Length];
		float pathLength = 0;
		
		for (int i = 0; i < path.Length; i++) {
			suppliedPath[i]=path[i].position;
		}
		
		Vector3[] vector3s = PathControlPointGenerator(suppliedPath);
		
		Vector3 prevPt = Interp(vector3s,0);
		int SmoothAmount = path.Length*20;
		for (int i = 1; i <= SmoothAmount; i++) {
			float pm = (float) i / SmoothAmount;
			Vector3 currPt = Interp(vector3s,pm);
			pathLength += Vector3.Distance(prevPt,currPt);
			prevPt = currPt;
		}
		
		return pathLength;
	}
	
	
	public static float PathLength(Vector3[] path){
		float pathLength = 0;
		
		Vector3[] vector3s = PathControlPointGenerator(path);
		
		//Line Draw:
		Vector3 prevPt = Interp(vector3s,0);
		int SmoothAmount = path.Length*20;
		for (int i = 1; i <= SmoothAmount; i++) {
			float pm = (float) i / SmoothAmount;
			Vector3 currPt = Interp(vector3s,pm);
			pathLength += Vector3.Distance(prevPt,currPt);
			prevPt = currPt;
		}
		
		return pathLength;
	}	

	public static void PutOnPath(GameObject target, Vector3[] path, float percent){
		target.transform.position=Interp(PathControlPointGenerator(path),percent);
	}
	
	public static void PutOnPath(Transform target, Vector3[] path, float percent){
		target.position=Interp(PathControlPointGenerator(path),percent);
	}	
	
	public static void PutOnPath(GameObject target, Transform[] path, float percent){
		//create and store path points:
		Vector3[] suppliedPath = new Vector3[path.Length];
		for (int i = 0; i < path.Length; i++) {
			suppliedPath[i]=path[i].position;
		}	
		target.transform.position=Interp(PathControlPointGenerator(suppliedPath),percent);
	}	
	
	public static void PutOnPath(Transform target, Transform[] path, float percent){
		//create and store path points:
		Vector3[] suppliedPath = new Vector3[path.Length];
		for (int i = 0; i < path.Length; i++) {
			suppliedPath[i]=path[i].position;
		}	
		target.position=Interp(PathControlPointGenerator(suppliedPath),percent);
	}		
	
	public static Vector3 PointOnPath(Transform[] path, float percent){
		//create and store path points:
		Vector3[] suppliedPath = new Vector3[path.Length];
		for (int i = 0; i < path.Length; i++) {
			suppliedPath[i]=path[i].position;
		}	
		return(Interp(PathControlPointGenerator(suppliedPath),percent));
	}
	
	
	public static Vector3 PointOnPath(Vector3[] path, float percent){
		return(Interp(PathControlPointGenerator(path),percent));
	}		
	
	
	public static void Resume(GameObject target){
		Component[] tweens = target.GetComponents<SliderTween>();
		foreach (SliderTween item in tweens){
			item.enabled=true;
		}
	}
	
	public static void Resume(GameObject target, bool includechildren){
		Resume(target);
		if(includechildren){
			foreach(Transform child in target.transform){
				Resume(child.gameObject,true);
			}			
		}
	}	
	
	public static void Resume(GameObject target, string type){
		Component[] tweens = target.GetComponents<SliderTween>();
		foreach (SliderTween item in tweens){
			string targetType = item.type+item.method;
			targetType=targetType.Substring(0,type.Length);
			if(targetType.ToLower() == type.ToLower()){
				item.enabled=true;
			}
		}
	}
	
	public static void Resume(GameObject target, string type, bool includechildren){
		Component[] tweens = target.GetComponents<SliderTween>();
		foreach (SliderTween item in tweens){
			string targetType = item.type+item.method;
			targetType=targetType.Substring(0,type.Length);
			if(targetType.ToLower() == type.ToLower()){
				item.enabled=true;
			}
		}
		if(includechildren){
			foreach(Transform child in target.transform){
				Resume(child.gameObject,type,true);
			}			
		}		
	}	
	
	public static void Resume(){
		for (int i = 0; i < tweens.Count; i++) {
			Hashtable currentTween = tweens[i];
			GameObject target = (GameObject)currentTween["target"];
			Resume(target);
		}
	}	
	
	public static void Resume(string type){
		ArrayList resumeArray = new ArrayList();
		
		for (int i = 0; i < tweens.Count; i++) {
			Hashtable currentTween = tweens[i];
			GameObject target = (GameObject)currentTween["target"];
			resumeArray.Insert(resumeArray.Count,target);
		}
		
		for (int i = 0; i < resumeArray.Count; i++) {
			Resume((GameObject)resumeArray[i],type);
		}
	}			
	
	public static void Pause(GameObject target){
		Component[] tweens = target.GetComponents<SliderTween>();
		foreach (SliderTween item in tweens){
			if(item.delay>0){
				item.delay-=Time.time-item.delayStarted;
				item.StopCoroutine("TweenDelay");
			}
			item.isPaused=true;
			item.enabled=false;
		}
	}
	
	public static void Pause(GameObject target, bool includechildren){
		Pause(target);
		if(includechildren){
			foreach(Transform child in target.transform){
				Pause(child.gameObject,true);
			}			
		}
	}	
	
	public static void Pause(GameObject target, string type){
		Component[] tweens = target.GetComponents<SliderTween>();
		foreach (SliderTween item in tweens){
			string targetType = item.type+item.method;
			targetType=targetType.Substring(0,type.Length);
			if(targetType.ToLower() == type.ToLower()){
				if(item.delay>0){
					item.delay-=Time.time-item.delayStarted;
					item.StopCoroutine("TweenDelay");
				}
				item.isPaused=true;
				item.enabled=false;
			}
		}
	}
	
	public static void Pause(GameObject target, string type, bool includechildren){
		Component[] tweens = target.GetComponents<SliderTween>();
		foreach (SliderTween item in tweens){
			string targetType = item.type+item.method;
			targetType=targetType.Substring(0,type.Length);
			if(targetType.ToLower() == type.ToLower()){
				if(item.delay>0){
					item.delay-=Time.time-item.delayStarted;
					item.StopCoroutine("TweenDelay");
				}
				item.isPaused=true;
				item.enabled=false;
			}
		}
		if(includechildren){
			foreach(Transform child in target.transform){
				Pause(child.gameObject,type,true);
			}			
		}		
	}	
	
	public static void Pause(){
		for (int i = 0; i < tweens.Count; i++) {
			Hashtable currentTween = tweens[i];
			GameObject target = (GameObject)currentTween["target"];
			Pause(target);
		}
	}	
	
	public static void Pause(string type){
		ArrayList pauseArray = new ArrayList();
		
		for (int i = 0; i < tweens.Count; i++) {
			Hashtable currentTween = tweens[i];
			GameObject target = (GameObject)currentTween["target"];
			pauseArray.Insert(pauseArray.Count,target);
		}
		
		for (int i = 0; i < pauseArray.Count; i++) {
			Pause((GameObject)pauseArray[i],type);
		}
	}		
	
	public static int Count(){
		return(tweens.Count);
	}
	
	public static int Count(string type){
		int tweenCount = 0;

		for (int i = 0; i < tweens.Count; i++) {
			Hashtable currentTween = tweens[i];
			string targetType = (string)currentTween["type"]+(string)currentTween["method"];
			targetType=targetType.Substring(0,type.Length);
			if(targetType.ToLower() == type.ToLower()){
				tweenCount++;
			}
		}	
		
		return(tweenCount);
	}			

	public static int Count(GameObject target){
		Component[] tweens = target.GetComponents<SliderTween>();
		return(tweens.Length);
	}
	
	public static int Count(GameObject target, string type){
		int tweenCount = 0;
		Component[] tweens = target.GetComponents<SliderTween>();
		foreach (SliderTween item in tweens){
			string targetType = item.type+item.method;
			targetType=targetType.Substring(0,type.Length);
			if(targetType.ToLower() == type.ToLower()){
				tweenCount++;
			}
		}
		return(tweenCount);
	}	
	
	
	public static void Stop(){
		for (int i = 0; i < tweens.Count; i++) {
			Hashtable currentTween = tweens[i];
			GameObject target = (GameObject)currentTween["target"];
			Stop(target);
		}
		tweens.Clear();
	}	
	
	public static void Stop(string type){
		ArrayList stopArray = new ArrayList();
		
		for (int i = 0; i < tweens.Count; i++) {
			Hashtable currentTween = tweens[i];
			GameObject target = (GameObject)currentTween["target"];
			stopArray.Insert(stopArray.Count,target);
		}
		
		for (int i = 0; i < stopArray.Count; i++) {
			Stop((GameObject)stopArray[i],type);
		}
	}		
	
	public static void StopByName(string name){
		ArrayList stopArray = new ArrayList();
		
		for (int i = 0; i < tweens.Count; i++) {
			Hashtable currentTween = tweens[i];
			GameObject target = (GameObject)currentTween["target"];
			stopArray.Insert(stopArray.Count,target);
		}
		
		for (int i = 0; i < stopArray.Count; i++) {
			StopByName((GameObject)stopArray[i],name);
		}
	}
	public static void Stop(GameObject target){
		Component[] tweens = target.GetComponents<SliderTween>();
		foreach (SliderTween item in tweens){
			item.Dispose();
		}
	}
	
	public static void Stop(GameObject target, bool includechildren){
		Stop(target);
		if(includechildren){
			foreach(Transform child in target.transform){
				Stop(child.gameObject,true);
			}			
		}
	}	
	
	public static void Stop(GameObject target, string type){
		Component[] tweens = target.GetComponents<SliderTween>();
		foreach (SliderTween item in tweens){
			string targetType = item.type+item.method;
			targetType=targetType.Substring(0,type.Length);
			if(targetType.ToLower() == type.ToLower()){
				item.Dispose();
			}
		}
	}
	 
	public static void StopByName(GameObject target, string name){
		Component[] tweens = target.GetComponents<SliderTween>();
		foreach (SliderTween item in tweens){
		 
			if(item._name == name){
				item.Dispose();
			}
		}
	}
 
	public static void Stop(GameObject target, string type, bool includechildren){
		Component[] tweens = target.GetComponents<SliderTween>();
		foreach (SliderTween item in tweens){
			string targetType = item.type+item.method;
			targetType=targetType.Substring(0,type.Length);
			if(targetType.ToLower() == type.ToLower()){
				item.Dispose();
			}
		}
		if(includechildren){
			foreach(Transform child in target.transform){
				Stop(child.gameObject,type,true);
			}			
		}		
	}
	
 
	public static void StopByName(GameObject target, string name, bool includechildren){
		Component[] tweens = target.GetComponents<SliderTween>();
		foreach (SliderTween item in tweens){
		 
			if(item._name == name){
				item.Dispose();
			}
		}
		if(includechildren){
			foreach(Transform child in target.transform){
				//Stop(child.gameObject,type,true);
				StopByName(child.gameObject,name,true);
			}			
		}		
	}
 
	public static Hashtable Set(params object[] setValues){
		Hashtable hashTable = new Hashtable(setValues.Length/2);
		if (setValues.Length %2 != 0) {
			Debug.LogError("Tween Error: Hash requires an even number of arguments!"); 
			return null;
		}else{
			int i = 0;
			while(i < setValues.Length - 1) {
				hashTable.Add(setValues[i], setValues[i+1]);
				i += 2;
			}
			return hashTable;
		}
	}	
	
	private SliderTween(Hashtable h) {
		tweenArguments = h;	
	}
	
	void Awake(){
		thisTransform = transform;
			
		RetrievesetValues();
        lastRealTime = Time.realtimeSinceStartup; // Added by PressPlay
	}
	
	IEnumerator Start(){
		if(delay > 0){
			yield return StartCoroutine("TweenDelay");
		}
		TweenStart();
	}	
	
	//non-physics
	void Update(){
		if(isRunning && !physics){
			if(!reverse){
				if(percentage<1f){
					TweenUpdate();
				}else{
					TweenComplete();	
				}
			}else{
				if(percentage>0){
					TweenUpdate();
				}else{
					TweenComplete();	
				}
			}
		}
	}
	
	//physics
	void FixedUpdate(){
		if(isRunning && physics){
			if(!reverse){
				if(percentage<1f){
					TweenUpdate();
				}else{
					TweenComplete();	
				}
			}else{
				if(percentage>0){
					TweenUpdate();
				}else{
					TweenComplete();	
				}
			}
		}	
	}

	void LateUpdate(){
		//look applications:
		if(tweenArguments.Contains("looktarget") && isRunning){
			if(type =="move" || type =="shake" || type=="punch"){
				LookUpdate(gameObject,tweenArguments);
			}			
		}
	}
	
	void OnEnable(){
		if(isRunning){
			EnableKinematic();
		}
	
		//resume delay:
		if(isPaused){
			isPaused=false;
			if(delay > 0){
				wasPaused=true;
				ResumeDelay();
			}
		}
	}

	void OnDisable(){
		DisableKinematic();
	}

	
	
	private static Vector3[] PathControlPointGenerator(Vector3[] path){
		Vector3[] suppliedPath;
		Vector3[] vector3s;
		
		//create and store path points:
		suppliedPath = path;

		//populate calculate path;
		int offset = 2;
		vector3s = new Vector3[suppliedPath.Length+offset];
		Array.Copy(suppliedPath,0,vector3s,1,suppliedPath.Length);
		
		//populate start and end control points:
		//vector3s[0] = vector3s[1] - vector3s[2];
		vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
		vector3s[vector3s.Length-1] = vector3s[vector3s.Length-2] + (vector3s[vector3s.Length-2] - vector3s[vector3s.Length-3]);
		
		//is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
		if(vector3s[1] == vector3s[vector3s.Length-2]){
			Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
			Array.Copy(vector3s,tmpLoopSpline,vector3s.Length);
			tmpLoopSpline[0]=tmpLoopSpline[tmpLoopSpline.Length-3];
			tmpLoopSpline[tmpLoopSpline.Length-1]=tmpLoopSpline[2];
			vector3s=new Vector3[tmpLoopSpline.Length];
			Array.Copy(tmpLoopSpline,vector3s,tmpLoopSpline.Length);
		}	
		
		return(vector3s);
	}
	
	private static Vector3 Interp(Vector3[] pts, float t){
		int numSections = pts.Length - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
		float u = t * (float) numSections - (float) currPt;
				
		Vector3 a = pts[currPt];
		Vector3 b = pts[currPt + 1];
		Vector3 c = pts[currPt + 2];
		Vector3 d = pts[currPt + 3];
		
		return .5f * (
			(-a + 3f * b - 3f * c + d) * (u * u * u)
			+ (2f * a - 5f * b + 4f * c - d) * (u * u)
			+ (-a + c) * u
			+ 2f * b
		);
	}	
		
	private class CRSpline {
		public Vector3[] pts;
		
		public CRSpline(params Vector3[] pts) {
			this.pts = new Vector3[pts.Length];
			Array.Copy(pts, this.pts, pts.Length);
		}
		
		
		public Vector3 Interp(float t) {
			int numSections = pts.Length - 3;
			int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
			float u = t * (float) numSections - (float) currPt;
			Vector3 a = pts[currPt];
			Vector3 b = pts[currPt + 1];
			Vector3 c = pts[currPt + 2];
			Vector3 d = pts[currPt + 3];
			return .5f*((-a+3f*b-3f*c+d)*(u*u*u)+(2f*a-5f*b+4f*c-d)*(u*u)+(-a+c)*u+2f*b);
		}	
	}	
	
	static void Launch(GameObject target, Hashtable setValues){
		if(!setValues.Contains("id")){
			setValues["id"] = GenerateID();
		}
		if(!setValues.Contains("target")){
			setValues["target"] = target;
		
		}		

		tweens.Insert (0, setValues);
		target.AddComponent<SliderTween>();
	}		
	
	static Hashtable CleansetValues(Hashtable setValues){
		Hashtable setValuesCopy = new Hashtable(setValues.Count);
		Hashtable setValuesCaseUnified = new Hashtable(setValues.Count);
		
		foreach (DictionaryEntry item in setValues) {
			setValuesCopy.Add(item.Key, item.Value);
		}
		
		foreach (DictionaryEntry item in setValuesCopy) {
			if(item.Value.GetType() == typeof(System.Int32)){
				int original = (int)item.Value;
				float casted = (float)original;
				setValues[item.Key] = casted;
			}
			if(item.Value.GetType() == typeof(System.Double)){
				double original = (double)item.Value;
				float casted = (float)original;
				setValues[item.Key] = casted;
			}
		}	
		
		//unify parameter case:
		foreach (DictionaryEntry item in setValues) {
			setValuesCaseUnified.Add(item.Key.ToString().ToLower(), item.Value);
		}	
		
		setValues = setValuesCaseUnified;
				
		return setValues;
	}	
	
	static string GenerateID(){
		int strlen = 15;
		char[] chars = {'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z','0','1','2','3','4','5','6','7','8'};
		int num_chars = chars.Length - 1;
		string randomChar = "";
		for (int i = 0; i < strlen; i++) {
		randomChar += chars[(int)Mathf.Floor(UnityEngine.Random.Range(0,num_chars))];
		}
		return System.Guid.NewGuid().ToString();
	}	
	
	void RetrievesetValues(){
		foreach (Hashtable item in tweens) {
			if((GameObject)item["target"] == gameObject){
				tweenArguments=item;
				break;
			}
		}
		
		id=(string)tweenArguments["id"];
		type=(string)tweenArguments["type"];
		_name=(string)tweenArguments["name"];
		method=(string)tweenArguments["method"];
               
		if(tweenArguments.Contains("time")){
			time=(float)tweenArguments["time"];
		}else{
			time=Defaults.time;
		}
			
		if(GetComponent<Rigidbody>() != null){
			physics=true;
		}
               
		if(tweenArguments.Contains("delay")){
			delay=(float)tweenArguments["delay"];
		}else{
			delay=Defaults.delay;
		}
		
		
		if(tweenArguments.Contains("looptype")){
			if(tweenArguments["looptype"].GetType() == typeof(LoopType)){
				loopType=(LoopType)tweenArguments["looptype"];
			}else{
				try {
					loopType=(LoopType)Enum.Parse(typeof(LoopType),(string)tweenArguments["looptype"],true); 
				} catch {
					Debug.LogWarning(" : Unsupported loopType supplied! Default will be used.");
					loopType = SliderTween.LoopType.none;	
				}
			}			
		}else{
			loopType = SliderTween.LoopType.none;	
		}			
         
		if(tweenArguments.Contains("easetype")){
			if(tweenArguments["easetype"].GetType() == typeof(EaseType)){
				easeType=(EaseType)tweenArguments["easetype"];
			}else{
				try {
					easeType=(EaseType)Enum.Parse(typeof(EaseType),(string)tweenArguments["easetype"],true); 
				} catch {
					Debug.LogWarning(" : Unsupported easeType supplied! Default will be used.");
					easeType=Defaults.easeType;
				}
			}
		}else{
			easeType=Defaults.easeType;
		}
				
		if(tweenArguments.Contains("space")){
			if(tweenArguments["space"].GetType() == typeof(Space)){
				space=(Space)tweenArguments["space"];
			}else{
				try {
					space=(Space)Enum.Parse(typeof(Space),(string)tweenArguments["space"],true); 	
				} catch {
					Debug.LogWarning(" : Unsupported space supplied! Default will be used.");
					space = Defaults.space;
				}
			}			
		}else{
			space = Defaults.space;
		}
		
		if(tweenArguments.Contains("islocal")){
			isLocal = (bool)tweenArguments["islocal"];
		}else{
			isLocal = Defaults.isLocal;
		}

        if (tweenArguments.Contains("ignoretimescale"))
        {
            useRealTime = (bool)tweenArguments["ignoretimescale"];
        }
        else
        {
            useRealTime = Defaults.useRealTime;
        }

		GetEasingFunction();
	}	
	
	void GetEasingFunction(){
		switch (easeType){
		case EaseType.easeInQuad:
			ease  = new EasingFunction(easeInQuad);
			break;
		case EaseType.easeOutQuad:
			ease = new EasingFunction(easeOutQuad);
			break;
		case EaseType.easeInOutQuad:
			ease = new EasingFunction(easeInOutQuad);
			break;
		case EaseType.easeInCubic:
			ease = new EasingFunction(easeInCubic);
			break;
		case EaseType.easeOutCubic:
			ease = new EasingFunction(easeOutCubic);
			break;
		case EaseType.easeInOutCubic:
			ease = new EasingFunction(easeInOutCubic);
			break;
		case EaseType.easeInQuart:
			ease = new EasingFunction(easeInQuart);
			break;
		case EaseType.easeOutQuart:
			ease = new EasingFunction(easeOutQuart);
			break;
		case EaseType.easeInOutQuart:
			ease = new EasingFunction(easeInOutQuart);
			break;
		case EaseType.easeInQuint:
			ease = new EasingFunction(easeInQuint);
			break;
		case EaseType.easeOutQuint:
			ease = new EasingFunction(easeOutQuint);
			break;
		case EaseType.easeInOutQuint:
			ease = new EasingFunction(easeInOutQuint);
			break;
		case EaseType.easeInSine:
			ease = new EasingFunction(easeInSine);
			break;
		case EaseType.easeOutSine:
			ease = new EasingFunction(easeOutSine);
			break;
		case EaseType.easeInOutSine:
			ease = new EasingFunction(easeInOutSine);
			break;
		case EaseType.easeInExpo:
			ease = new EasingFunction(easeInExpo);
			break;
		case EaseType.easeOutExpo:
			ease = new EasingFunction(easeOutExpo);
			break;
		case EaseType.easeInOutExpo:
			ease = new EasingFunction(easeInOutExpo);
			break;
		case EaseType.easeInCirc:
			ease = new EasingFunction(easeInCirc);
			break;
		case EaseType.easeOutCirc:
			ease = new EasingFunction(easeOutCirc);
			break;
		case EaseType.easeInOutCirc:
			ease = new EasingFunction(easeInOutCirc);
			break;
		case EaseType.linear:
			ease = new EasingFunction(linear);
			break;
		case EaseType.spring:
			ease = new EasingFunction(spring);
			break;
	
		case EaseType.easeInBounce:
			ease = new EasingFunction(easeInBounce);
			break;
		case EaseType.easeOutBounce:
			ease = new EasingFunction(easeOutBounce);
			break;
		case EaseType.easeInOutBounce:
			ease = new EasingFunction(easeInOutBounce);
			break;
		case EaseType.easeInBack:
			ease = new EasingFunction(easeInBack);
			break;
		case EaseType.easeOutBack:
			ease = new EasingFunction(easeOutBack);
			break;
		case EaseType.easeInOutBack:
			ease = new EasingFunction(easeInOutBack);
			break;
		
		case EaseType.easeInElastic:
			ease = new EasingFunction(easeInElastic);
			break;
		case EaseType.easeOutElastic:
			ease = new EasingFunction(easeOutElastic);
			break;
		case EaseType.easeInOutElastic:
			ease = new EasingFunction(easeInOutElastic);
			break;

		}
	}
	
	void UpdatePercentage(){

	        if (useRealTime)
	        {
	            runningTime += (Time.realtimeSinceStartup - lastRealTime);      
	        }
	        else
	        {
	            runningTime += Time.deltaTime;
	        }
	
			if(reverse){
				percentage = 1 - runningTime/time;	
			}else{
				percentage = runningTime/time;	
			}
	
	        lastRealTime = Time.realtimeSinceStartup; 
	}
	
	void CallBack(string callbackType){
		if (tweenArguments.Contains(callbackType) && !tweenArguments.Contains("ischild")) {
			GameObject target;
			if (tweenArguments.Contains(callbackType+"target")) {
				target=(GameObject)tweenArguments[callbackType+"target"];
			}else{
				target=gameObject;	
			}
			
			if (tweenArguments[callbackType].GetType() == typeof(System.String)) {
				target.SendMessage((string)tweenArguments[callbackType],(object)tweenArguments[callbackType+"params"],SendMessageOptions.DontRequireReceiver);
			}else{
				Debug.LogError("  Error: Callback method references must be passed as a String!");
				Destroy (this);
			}
		}
	}
	
	void Dispose(){
		for (int i = 0; i < tweens.Count; i++) {
			Hashtable tweenEntry = tweens[i];
			if ((string)tweenEntry["id"] == id){
				tweens.RemoveAt(i);
				break;
			}
		}
		Destroy(this);
	}	
	
	void ConflictCheck(){
		Component[] tweens = GetComponents<SliderTween>();
		foreach (SliderTween item in tweens) {
			if(item.type == "value"){
				return;
			}else if(item.isRunning && item.type==type){
				if (item.method != method) {
					return;
				}				
				
				if(item.tweenArguments.Count != tweenArguments.Count){
					item.Dispose();
					return;
				}
				
				foreach (DictionaryEntry currentProp in tweenArguments) {
					if(!item.tweenArguments.Contains(currentProp.Key)){
						item.Dispose();
						return;
					}else{
						if(!item.tweenArguments[currentProp.Key].Equals(tweenArguments[currentProp.Key]) && (string)currentProp.Key != "id"){
							item.Dispose();
							return;
						}
					}
				}
				Dispose();
			}
		}
	}
	
	void EnableKinematic(){
		
	}
	
	void DisableKinematic(){
	}
		
	void ResumeDelay(){
		StartCoroutine("TweenDelay");
	}	
	
	private float linear(float start, float end, float value){
		return Mathf.Lerp(start, end, value);
	}
	
	private float clerp(float start, float end, float value){
		float min = 0.0f;
		float max = 360.0f;
		float half = Mathf.Abs((max - min) * 0.5f);
		float retval = 0.0f;
		float diff = 0.0f;
		if ((end - start) < -half){
			diff = ((max - start) + end) * value;
			retval = start + diff;
		}else if ((end - start) > half){
			diff = -((max - end) + start) * value;
			retval = start + diff;
		}else retval = start + (end - start) * value;
		return retval;
    }

	private float spring(float start, float end, float value){
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}

	private float easeInQuad(float start, float end, float value){
		end -= start;
		return end * value * value + start;
	}

	private float easeOutQuad(float start, float end, float value){
		end -= start;
		return -end * value * (value - 2) + start;
	}

	private float easeInOutQuad(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * value * value + start;
		value--;
		return -end * 0.5f * (value * (value - 2) - 1) + start;
	}

	private float easeInCubic(float start, float end, float value){
		end -= start;
		return end * value * value * value + start;
	}

	private float easeOutCubic(float start, float end, float value){
		value--;
		end -= start;
		return end * (value * value * value + 1) + start;
	}

	private float easeInOutCubic(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * value * value * value + start;
		value -= 2;
		return end * 0.5f * (value * value * value + 2) + start;
	}

	private float easeInQuart(float start, float end, float value){
		end -= start;
		return end * value * value * value * value + start;
	}

	private float easeOutQuart(float start, float end, float value){
		value--;
		end -= start;
		return -end * (value * value * value * value - 1) + start;
	}

	private float easeInOutQuart(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * value * value * value * value + start;
		value -= 2;
		return -end * 0.5f * (value * value * value * value - 2) + start;
	}

	private float easeInQuint(float start, float end, float value){
		end -= start;
		return end * value * value * value * value * value + start;
	}

	private float easeOutQuint(float start, float end, float value){
		value--;
		end -= start;
		return end * (value * value * value * value * value + 1) + start;
	}

	private float easeInOutQuint(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * value * value * value * value * value + start;
		value -= 2;
		return end * 0.5f * (value * value * value * value * value + 2) + start;
	}

	private float easeInSine(float start, float end, float value){
		end -= start;
		return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
	}

	private float easeOutSine(float start, float end, float value){
		end -= start;
		return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
	}

	private float easeInOutSine(float start, float end, float value){
		end -= start;
		return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
	}

	private float easeInExpo(float start, float end, float value){
		end -= start;
		return end * Mathf.Pow(2, 10 * (value - 1)) + start;
	}

	private float easeOutExpo(float start, float end, float value){
		end -= start;
		return end * (-Mathf.Pow(2, -10 * value ) + 1) + start;
	}

	private float easeInOutExpo(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
		value--;
		return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
	}

	private float easeInCirc(float start, float end, float value){
		end -= start;
		return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
	}

	private float easeOutCirc(float start, float end, float value){
		value--;
		end -= start;
		return end * Mathf.Sqrt(1 - value * value) + start;
	}

	private float easeInOutCirc(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
		value -= 2;
		return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
	}

	private float easeInBounce(float start, float end, float value){
		end -= start;
		float d = 1f;
		return end - easeOutBounce(0, end, d-value) + start;
	}
	private float easeOutBounce(float start, float end, float value){
		value /= 1f;
		end -= start;
		if (value < (1 / 2.75f)){
			return end * (7.5625f * value * value) + start;
		}else if (value < (2 / 2.75f)){
			value -= (1.5f / 2.75f);
			return end * (7.5625f * (value) * value + .75f) + start;
		}else if (value < (2.5 / 2.75)){
			value -= (2.25f / 2.75f);
			return end * (7.5625f * (value) * value + .9375f) + start;
		}else{
			value -= (2.625f / 2.75f);
			return end * (7.5625f * (value) * value + .984375f) + start;
		}
	}
	private float easeInOutBounce(float start, float end, float value){
		end -= start;
		float d = 1f;
		if (value < d* 0.5f) return easeInBounce(0, end, value*2) * 0.5f + start;
		else return easeOutBounce(0, end, value*2-d) * 0.5f + end*0.5f + start;
	}

	private float easeInBack(float start, float end, float value){
		end -= start;
		value /= 1;
		float s = 1.70158f;
		return end * (value) * value * ((s + 1) * value - s) + start;
	}

	private float easeOutBack(float start, float end, float value){
		float s = 1.70158f;
		end -= start;
		value = (value) - 1;
		return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
	}

	private float easeInOutBack(float start, float end, float value){
		float s = 1.70158f;
		end -= start;
		value /= .5f;
		if ((value) < 1){
			s *= (1.525f);
			return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
		}
		value -= 2;
		s *= (1.525f);
		return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
	}

	private float punch(float amplitude, float value){
		float s = 9;
		if (value == 0){
			return 0;
		}
		else if (value == 1){
			return 0;
		}
		float period = 1 * 0.3f;
		s = period / (2 * Mathf.PI) * Mathf.Asin(0);
		return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
    }
	
	private float easeInElastic(float start, float end, float value){
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d) == 1) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p / 4;
			}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		return -(a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
	}		
	private float easeOutElastic(float start, float end, float value){
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d) == 1) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p * 0.25f;
			}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
	}		
	
	private float easeInOutElastic(float start, float end, float value){
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d*0.5f) == 2) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p / 4;
			}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
		return a * Mathf.Pow(2, -10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
	}		
	
}

