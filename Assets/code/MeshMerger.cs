// Mesh Merger Script
// Copyright 2009, Russ Menapace
// http://humanpoweredgames.com

// Summary:
//  This script allows you to draw a large number of meshes with a single 
//  draw call.  This is particularly useful for iPhone games.

// License:
//  Free to use as you see fit, but I would appreciate one of the following:
//  * A credit for Human Powered Games, or even a link to humanpoweredgames.com
//    in whatever you make with this
//  * Hire me to make games or simulations
//  * A donation to the PayPal account at russ@databar.com.  I'm very poor, so 
//    even a small donation would be greatly appreciated!
//  * A thank you note to russ@databar.com
//  * Suggestions on how the script could be improved mailed to russ@databar.com

// Warranty:
//  This software carries no warranty, and I don't guarantee anything about it.
//  If it burns down your house or gets your cat pregnant, don't look at me. 

// Acnowledgements: 
//  This was pieced together out of code I found onthe UnifyCommunity wiki, and 
//  the Unity forum.  I did not keep track of names, but I do recall gaining
//  a lot of insight from the posts of PirateNinjaAlliance.  
//  Thanks to anybody that may have been involved.

// Requirements:  
//  All the meshes you want to use must use the same material.  
//  This material may be a texture atlas and the meshes UV to portions of the atlas.
//  The texture atlas technique works particularly well for GUI stuff.

// Usage:
//  There are two ways to use this script:

//  Implicit:  
//    Simply drop the script into a GameObject that has a number of
//    child objects containing mesh filters.

//  Explicit:
//    Populate the meshFilter array with the meshes you want merged
//    Optionally, set the material to be used.  If no material is selected,
//    The script will apply the first material it encounters to all subsequent
//    meshes

// To see if it's working:
//  Move the camera so you can see several of your objects in the Game pane
//  Note the number of draw calls
//  Hit play. You should see the number of draw calls for those meshes reduced to one

using UnityEngine;
using System;
//==============================================================================
public class MeshMerger : MonoBehaviour 
{

}