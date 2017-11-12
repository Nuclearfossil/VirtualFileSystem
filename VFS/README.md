# Virtual File System

## Summary

There are a number of Virtual file systems out there, but I wanted to take a stab
at building my own. Why fall into the trap of NIH?

1. I wanted a bit finer control over what was available. For the most part, I'm only interested in R/O operations.
2. I'm not interested in support for Web/networked based File Systems.
3. I want to support Zip with a potentially different set of compressors
4. I also want to build my own pak file format. This leads into that operation well.

## Design

### Node

At it's core, we essentially have a file/directory 'Node'. This isn't a unix 'INode', it's just an item
in the file system; a file or a directory. Each Node can be flagged as ReadOnly as well as binary. You can
derive from Node, but for the most part, you shouldn't have to, unless the underly System requires you to
persist some extra data (like 'ZipNode')

### INodeTree

This isn't a tree structure. It maps to the tree of the underlying file system with a flat list. This has 
the side effect of not having a fast directory parsing structure (you need to iterate over the whole file system
in order to find files in a specific subdirectory). I may add some helper functions in the near future, or revisit
the system to add a hierarchy at a later point in time.

INodeTree is an interface; you need to build concrete classes in order to represent specific file systems. For
example, look at the LocalFileSystem and the ZipFileSystem for concrete classes.

### Repository

This is the highest level system that you will want to interact with. This is meant to wrap your access to files
in a more abstract way. The best way to describe it is to give an example:

Say you have files that look something like this:

    C:\project\data\Folder01
    D:\localdatastore\data\Folder02
    C:\project\data\data.zip

What you are trying to do here is map a zip file and two directories as a file store. We don't actually care what is in the folders or the zip file.
You don't actively care to keep track of where files are, but you want to be able to access them as if it was one file system.

ie:

```
fileSystem.Open(@"alphabetagamma\db.sample.txt"))
```

All we need to know is that in one of those folders, or in the zip file, there should exist a file, in a folder called
`alphabetagamma` called `db.sample.txt`.

How does the Repository figure out which file to load if there are multiple instances of the same file? Fairly simply - when you add an INodeTree into
the repository, the last one in is the first one it uses. It's not clever and probably the least flexible option, but it should be painfully obvious in
it's logic.

I do allow writing of files, but it is far from elegant. It follows the same logic as reading files, except that it checks to see if you can actually write
to the file system before trying to use it.

## How to use it

The best way to figure out how to use this system is to look at the unit tests. They're fairly simple and straightforward.

## Roadmap

I'm still pondering how best to deal with Revision Control software, like Perforce. I'd like that to just be an extension when writing files. But I want to
hammer on this a bit more before we go down that path.

It is very light on catching/throwing exceptions.

I would like to support additional file system types (specifically other file compression formats).