**************************************
*          HIGHLIGHT PLUS 2D         *
* Created by Ramiro Oliva (Kronnect) * 
*            README FILE             *
**************************************


>> Please read the documentation included in "Documentation" folder for instructions.


Support & Feedback
------------------

If you like Highlight Plus 2D, please rate it on the Asset Store. It encourages us to keep improving it! Thanks!

Have any question or issue?
* Email: contact@kronnect.com
* Support Forum: http://kronnect.com
* Twitter: @KronnectGames



Future updates
--------------

All our assets follow an incremental development process by which a few beta releases are published on our support forum (kronnect.com).
We encourage you to signup and engage our forum. The forum is the primary support and feature discussions medium.

Of course, all updates of Highlight Plus 2D will be eventually available on the Asset Store.



More Cool Assets!
-----------------
Check out our other assets here:
https://assetstore.unity.com/publishers/15018



Version history
---------------

Version 3.5.2 10/May/2021
- Fixes and internal improvements

Version 3.5.1 29/Apr/2021
- Fixed regression with quad-based sprites

Version 3.5
- Suppport for multiple sprite skin components in a hierarchy
- [Fix] Fixed Highlight Groud 2D shader file

Version 3.4.1 13-MAR-2021
- Material setters optimizations
- [Fix] Fixes and minor improvements

Version 3.4 21-FEB-2021
- Added "Include" option which allows you to specify the group highlight behaviour (only this object, include children, etc)
- [Fix] Fixed an issue when manually copying values from one component to another by which the highlight target was also copied causing the second object to highlight the first object

Version 3.3 17-FEB-2021
- Added support for 2D animation (read documentation for instructions)

Version 3.2 29-SEP-2020
- Improved compatibility with existing sprite masks
- Setup material optimizations

Version 3.1.1
- Improve performance of group checks
- API: exposed UpdateMaterialProperties for quick update of properties from scripting

Version 3.1
- Added "Hit FX" effect (use from scripting)
- Faster and less memory allocations during initialization

Version 3.0.1
- [Fix] Fixed prefab marked dirty when preview in editor option is enabled

Version 3.0
- Added GPU instancing support to glow effect
- New "Highlight Plus 2D Group" component. Can be added to effects to group outline effects
- Support for SVG sprites. Enable "Polygon/SVG" toggle in inspector

Version 2.3
- Added "Outline Exclusive" option (shows outline regardless of other overlapping highlighted sprites)

Version 2.2.1
- Improved outline effect when Smooth Edge is used on semi-transparent sprites
- Minor fixes
- [Fix] Fixed demo sprite sheet bleeding issue

Version 2.1
- Improved "Smooth Edges" option
- Outline / Glow quality now offers 3 levels

Version 2.0
- Added support for polygon packed sprites
- Added "Render On Top" option to glow and outline
- Added "Smooth Edges" option to glow and outline
- [Fix] Fixed alpha issue with 2D shadow effect 

Version 1.8
- Added "Outline Always On Top" and "Glow Always On Top" options

Version 1.7.1
- [Fix] Fixed non-uniform outline widths in different parts of a hierarchy of sprites

Version 1.7
- Added "Scale" option
- [Fix] Fixed occluder option not rendering when no other effect is enabled

Version 1.6
- Added "Occluder" option. Forces sprite to write to z-buffer causing occlusion to other sprites (enables see-through)
- [Fix] Fixed demo scene for LWRP

Version 1.5
- Added "Shadow" effect

Version 1.4
- Changed rendering method to support outline occlusion
- Added compatibility with LWRP (Lightweight Rendering Pipeline)
- Added "Highlight Event" and "Highlight Duration" to trigger and manager
- Added "Overlay RenderQueue" parameter
- API: added OnObjectHighlightStart, ObObjectHighlightEnd events to Highlight Manager 2D component

Version 1.3
- Added "Raycast Source" to Highlight Trigger and Manager

Version 1.2
- Added support for quad-based sprites

Version 1.1
- Added "Overlay Min Intensity" and "Overlay Blending" options
- Added "Glow HQ" option
- [Fix] Clicking Reset command does not refresh properly

Version 1.0
- Initial release
