using UnityEngine;
using System.Collections;

namespace Voltage.AttributeTools {

	/// <summary>
	/// Adding comments to GameObjects in the Inspector.
	/// </summary>
	public class Comments : MonoBehaviour {
	
		/// <summary>
		/// The comment.
		/// </summary>
		[Multiline]
		public string text;
	}
}
