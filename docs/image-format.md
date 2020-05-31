## Image Data
The image data is variable in size.  It starts off with a CRC32 checksum, followed by a 40 byte header followed by the pixel data.  The image header starts with 24 bytes used by NFL Blitz, followed by 16 bytes used by that are the Glide specific struct of `GrTexInfo`.  Detailed technical information can be found in [the Glide Reference Manual](https://3dfxglide.com/download/GL3REF.PDF).

| Position | Size (bytes) | Purpose | `Image` Property |
|----------|--------------|---------|------------------|
| 0 | 4 | Image header version.  Always set 0x8005 | `Version`|
| 4 | 4 | Referred to as Bias in the code, but it's purpose is unknown.  May have something to do with 3rd party copyright. | `Bias` |
| 8 | 4 | Filter mode used to apply mip maps | `FilterMode` |
| 12 | 4 | Indicates if Trilinear Filtering should be used | `UseTrilinearFiltering`
| 16 | 4 | Width of image | `Width` |
| 20 | 4 | Height of image | `Height` |
| 24 | 4 | Smallest Mipmapping level | `SmallestLOD` |
| 28 | 4 | Largest Mipmapping level | `LargestLOD` |
| 32 | 4 | The Aspect Ratio of the images in the texture | `AspectRatio` |
| 36 | 4 | The pixel format used in this image | `ImageFormat` |
| 40 | (variable) | The actual pixel data | `Data` |

## Images
The image in the NFL Blitz file are a basic set of pixels in a specific color format.  These color formats compress 32-bit color values into 8-bit or 16-bit storage.  Not all image formats supported by 3DFX are used by NFL Blitz and only those used are listed below.
| Id | Format | Description |
|----|--------|-------------|
| 00 | `RGB332` | A single byte value with Red and Green using 3 bits and Blue using 2 bits. |
| 08 | `ARGB8332` | Color is stored in a 16 bit value with the first byte representing the alpha channel and the remaining by following the same format as `RGB332`.
| 0a | `RGB565` | Red and Blue colors are stored as 5 bit value, Green is stored as a 6 bit color.  There is no Alpha color. |
| 0b | `RGBA1555` | Red, Green, and Blue are stored as 5 bit values, Alpha channel is a single 1 bit value |
| 0c | `RBGA4444` | Red, Green, Blue, and Alpha are stored as a 4 bit value |
| 0d | `AlphaIntensity16` | Image stores the alpha levels information as a 16-bit value |

## Record Reader
Currently, a single class, `ImageDataReader`, provides functionality around reading image records from a stream.