## Images
The image in the NFL Blitz file are a basic set of pixels in a specific color format.  These formats represent pixel 
data in a 16-bit block.
| Id | Format | Description |
|----|--------|-------------|
| 00 | BitmapMask? | A single byte value.  Looks to store an alpha value and used to act like a mask |
| 03 | ?? | |
| 0a | RGB565 | Red and Blue colors are stored as 5 bit value, Green is stored as a 6 bit color.  There is no Alpha color. |
| 0b | RGBA1555 | Red, Green, and Blue are stored as 5 bit values, Alpha channel is a single 1 bit value |
| 0c | RBGA4444 | Red, Green, Blue, and Alpha are stored as a 4 bit value |
| 0d | ?? | |

### Image Record
The image record is variable in size.  It starts off with a 40 byte header followed by the pixel data.

| Position | Size (bytes) | Purpose | `Image` Property |
|----------|--------------|---------|------------------|
| 0 | 4 | ?? - File Type? Always set to 0x8005 to indicate it's an image? | `FileType`|
| 4 | 4 | ?? - Reserved? | `Reserved` |
| 8 | 4 | ?? - Unknown Value | `UnknownValue1` |
| 12 | 4 | ?? | `UnknownValue2`
| 16 | 4 | Width of image | `Width` |
| 20 | 4 | Height of image | `Height` |
| 24 | 4 | Mipmapping level (0, 2, 4, 8).  This is used by images that need multiple levels of detail (LOD) | `MipmappingLevel` |
| 28 | 4 | ?? | `UnknownValue4` |
| 32 | 4 | ?? | `UnknownValue5` |
| 36 | 4 | The pixel format used in this image | `ImageFormat` |
| 40 | `Width` * `Height` * 2 | The actual pixel data | `Data` |
