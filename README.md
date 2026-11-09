# Relink SDK

A repository containing a bunch of utilities, structures and other elements that I use across all of my Reloaded-II code mods. Most of it was found with Ghidra, learned from other mods, and trial and error.

This isn't a Reloaded-II mod by itself, and is meant to be used in C# projects that reference it.

It's also kind of overengineered, mostly to avoid writing/duplicating structures/properties. It has an entire bindings generator. Anyone however, is free to use this SDK if they see fit.

## Bindings

Current bindings target **2.0.6**; any other version is rejected at startup.

Class names come from the game's own RTTI wherever I can find one. All of the RVAs/offsets were either found by myself, or referenced/used from other repositories and mods that found them before me.
