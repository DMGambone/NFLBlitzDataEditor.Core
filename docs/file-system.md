# Midway File System
The hard drives in the Midway games contain what is a customized version of FAT32.  The hard drive uses 512 byte sectors.  The file system will store data in 4096 byte blocks of data, which are usually called `clusters`. A cluster is the smallest logical unit in the file system and files are often stored in multiple clusters.  When files are saved, the space allocate would be rounded up to the nearest cluster size.

The file system clusters are calculated based on a starting base address using the formula of `FirstAllocationTableAddress - (3 * ClusterSize)`.  The reason for the offset is still not yet known.  The location of Cluster `C` is `BaseStartingAddress + (C * Clustersize)`.

## File System Header
Located in the 4th sector (in NFL Blitz 2k, other games differ) is a 48 byte file system header.  Not everything is known about this file system header yet, but some information has been figured out:
| Position | Size | Description |
|----------|------|-------------|
| 0  | 4 | ?? | 
| 4  | 4 | Number of sectors per cluster | 
| 8  | 4 | Sector index of cluster table | 
| 12 | 4 | Sector index of first file allocation table |
| 16 | 4 | ?? | 
| 20 | 4 | ?? | 
| 24 | 4 | ?? | 
| 28 | 4 | ?? | 
| 32 | 4 | ?? | 
| 36 | 4 | ?? | 
| 40 | 4 | ?? | 
| 44 | 4 | ?? | 
| 48 | 4 | ?? | 

## Cluster Table
The file system has a table that contains a mapping of clusters to actual physical clusters.  Since the file system is compiled to be read only, it's compact and unfragmented so the use of the cluster table is not really needed.  However, for completeness, the cluster table is referenced in the `MidwayFS` when reading files.  When writing back to the file system, the cluster table does need to be updated so that the game reads it properly.

## File System Partitions
The file system is broken down into multiple sections that are referred to as partitions.

A single file system partition is broken down into 2 basic sections: File Allocation Table and File Data.  The File Allocation Table is a single cluster in length, which is large enough to store 170 file entries and a pointer to the next partition.

| Position | Size | Description |
|----------|------|-------------|
| 0 | 4080 | The partition file allocation table | 
| 4080 | 12 | Reserved | 
| 4092 | 4 | The location of the next partition, 0 indicating there are no more partitions | 
| 4096 | variable | The actual file data |

The location of the first partition is defines in the file system header, which is located in the 4th sector (actual address = 0x600).

## Partition File Allocation Table Entries
Each partition contains a file allocation table which breaks down basic information about the files within the partition.  Each entry is a 24 byte record:

| Position | Size | Description |
|----------|------|-------------|
| 0 | 12 | The filename stored as a null-terminated string.  <sup>1</sup> | 
| 12 | 4 | The file size as a multiple of 32-bit words.  The actual file size is the number * 4. | 
| 16 | 4 | The timestamp of the file. | 
| 20 | 4 | The starting cluster index within the cluster table. |

<sup>1</sup> The filename is stored 3 big-endian 32-bit words.  Converting these to strings requires the words to be individually flipped.  For example, `GAME.EXE` is stored as `EMAGEXE.`.


### File timestamp
The timestamp is stored as a 32 bit value.  The packed structure looks like the following:
| bits | Description |
|------|-------------|
| 5    | hours       |
| 6    | minutes     |
| 5    | seconds <sup>1</sup>   |
| 7    | year        |
| 4    | month       |
| 5    | day         |

<sup>1</sup> Seconds is a value from 0 to 31, so seconds in the timestamp is multiplied by 2 as more precision is not needed.

## IDataBuffer
In order to complete abstract how data is actually read from the source, the `FileSystem` class uses a basic abstraction class called `IDataBuffer`.  Implementations of `IDataBuffer` can be written so that the data can be read/written directly against any type of source, such as the physical drive, against CHD file, the extracted contents of the CHD file, or any other ways. 