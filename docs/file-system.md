# Midway File System
The hard drives in the Midway games contain what appears to be a custom file system.  The hard drive uses 512 byte sectors.  The file system will store data in 4096 byte blocks. The allocated size of the file on the drive is always rounded up to the nearest 4k boundary.  So a small 32 byte file would use 4096 bytes.

Calculating the location of a sector `X` is simply `(X - 1) * SectorSize`.

The file system blocks are calculated based on an starting base address.  The location of a block `B` is `BaseStartingAddress + (B * BlockSize)`.

## File System Partition
The file system is broken down into partitions.  Each partition can contain up to 170 files in it.

A single file system partition is broken down into 3 basic sections:

| Position | Size | Description |
|----------|------|-------------|
| 0 | 4080 | The partition file allocation table | 
| 4080 | 12 | Reserved | 
| 4092 | 4 | The size of the partition | 
| 4096 | variable | The actual file data |

The location of the first partition is defines in the file system header, which is located in the 4th sector (actual address = 0x600).

## Partition File Allocation Table Entries
Each partition contains a file allocation table which breaks down basic information about the files within the partition.  Each entry is a 24 byte record:

| Position | Size | Description |
|----------|------|-------------|
| 0 | 12 | The filename stored as a null-terminated string.  <sup>1</sup> | 
| 12 | 4 | The file size as a multiple of 32-bit words.  The actual file size is the number * 4. | 
| 16 | 4 | The timestamp of the file. | 
| 20 | 4 | The starting block index within the file system. |

<sup>1</sup> The filename is stored 3 big-endian 32-bit words.  Converting these to strings requires the words to be individually flipped.  For example, `GAME.EXE` is stored as `EMAGEXE.`.

### File timestamp
The timestamp is stored as a 32 bit value.  The packed structure looks like the following:
| bits | Description |
|------|---------|
| 5    | hours   |
| 6    | minutes   |
| 5    | seconds (value is multiplied by 2)   |
| 7    | year   |
| 4    | month   |
| 5    | day   |

## IDataBuffer
In order to complete abstract how data is actually read from the hard drive, `FileSystem` uses a basic abstraction class called `IDataBuffer`.  With this implementation, implementations of `IDataBuffer` can be writte so that the data can be read/written directly against the physical drive, against CHD file, the extracted contents of the CHD file, or any other ways. 