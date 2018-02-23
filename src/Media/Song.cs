#region License
/* FNA - XNA4 Reimplementation for Desktop Platforms
 * Copyright 2009-2018 Ethan Lee and the MonoGame Team
 *
 * Released under the Microsoft Public License.
 * See LICENSE for details.
 */
#endregion

#region Using Statements
using System;
#endregion

namespace Microsoft.Xna.Framework.Media
{
	public sealed class Song : IEquatable<Song>, IDisposable
	{
		#region Public Properties

		public string Name
		{
			get;
			private set;
		}

		public TimeSpan Duration
		{
			get;
			private set;
		}

		public bool IsProtected
		{
			get
			{
				return false;
			}
		}

		public bool IsRated
		{
			get
			{
				return false;
			}
		}

		public int PlayCount
		{
			get;
			internal set;
		}

		public int Rating
		{
			get
			{
				return 0;
			}
		}

		public int TrackNumber
		{
			get
			{
				return 0;
			}
		}

		#endregion

		#region Public IDisposable Properties

		public bool IsDisposed
		{
			get;
			private set;
		}

		#endregion

		#region Internal Variables

		internal IntPtr handle;

		#endregion

		#region Constructors, Deconstructor, Dispose()

		internal Song(string fileName)
		{
			float seconds;
			handle = FAudio.XNA_GenSong(fileName, out seconds);
			if (handle == IntPtr.Zero)
			{
				throw new Audio.NoAudioHardwareException();
			}
			Duration = TimeSpan.FromSeconds(seconds);
			IsDisposed = false;
		}

		internal Song(string fileName, int durationMS) : this(fileName)
		{
			/* If you got here, you've still got the XNB file! Well done!
			 * Except if you're running FNA, you're not using the WMA anymore.
			 * But surely it's the same song, right...?
			 * Well, consider this a check more than anything. If this bothers
			 * you, just remove the XNB file and we'll read the OGG straight up.
			 *
			 * FIXME: Guess what, durationMS isn't terribly accurate, so forget it.
			 * -flibit
			if (Math.Abs(Duration.Milliseconds - durationMS) > 1000)
			{
				throw new InvalidOperationException("XNB/OGG duration mismatch!");
			}
			 */
		}

		~Song()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (!IsDisposed)
			{
				FAudio.XNA_DisposeSong(handle);
				handle = IntPtr.Zero;
				IsDisposed = true;
			}
		}

		#endregion

		#region Public Comparison Methods/Operators

		public bool Equals(Song song) 
		{
			return (((object) song) != null) && (Name == song.Name);
		}

		public override bool Equals(Object obj)
		{
			if (obj == null)
			{
				return false;
			}
			return Equals(obj as Song);
		}

		public static bool operator ==(Song song1, Song song2)
		{
			if (((object) song1) == null)
			{
				return ((object) song2) == null;
			}
			return song1.Equals(song2);
		}

		public static bool operator !=(Song song1, Song song2)
		{
			return !(song1 == song2);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Constructs a new Song object based on the specified URI.
		/// </summary>
		/// <remarks>
		/// This method matches the signature of the one in XNA4, however we currently can't play remote songs, so
		/// the URI is required to be a file name and the code uses the LocalPath property only.
		/// </remarks>
		/// <param name="name">Name of the song.</param>
		/// <param name="uri">Uri object that represents the URI.</param>
		/// <returns>Song object that can be used to play the song.</returns>
		public static Song FromUri(string name, Uri uri)
		{
			string path;
			if (uri.IsAbsoluteUri)
			{
				// If it's absolute, be sure we can actually get it...
				if (!uri.IsFile)
				{
					throw new InvalidOperationException(
						"Only local file URIs are supported for now"
					);
				}
				path = uri.LocalPath;
			}
			else
			{
				path = uri.ToString();
			}

			return new Song(path)
			{
				Name = name
			};
		}

		#endregion
	}
}
