# ChangeLog

> Adelic Systems(c) Asset-base AI Rule System and Rule System Editors.
> All notable system upgrades will be tracked here.

### **1.0.6**

#### [ADDED]
- Composite Action Elements.
	> Rule System now provides Composite Action Services. It is possible to sequence or Compound multiple actions (or other Composites) in larger/Complex Actions.

#### [CHANGED]
- RuleController Services
	> Provided some tracking services to accompany Action Composites.
- Liberary Window Visual Feedback
	- Small visual fb change regarding distinction between  Prefabs and Instances in Library.

#### [FIXED]
- Mutators are now seen as decisions.

### **1.0.5**

#### [ADDED]
- Behavior Profile Services:
	
	> Profiles now have their first public service functions. This allows for scripted (and thus Runtime) manipulation of Rulesets.
- Inspector friendliness
	- Rule Property Drawer
	> It is now possible to open any Rule (either in a Profile or Individual) in the rule editor window by the press of a button in the Inspector.


###  **1.0.2**

#### [CHANGED]
- Visibility on Active Rules and Potential rules, Useful for Debugging.

#### [FIXED]
- Rule Controller:
	-Quality relation between Active and Potential Rules.

### **1.0.1**
#### [ADDED]
- RuleController:
	- IsRuleActive public function
		
		> RuleControllers are now able to check if a rule is already active.

### **1.0.0**
#### [ADDED]
- Build-In Assets for Inequality operations.
- Small visual touch ups.
- Ability to set Generic Set names in Library.

#### [CHANGED]
- Rect size of Action.
- Asset loading.

#### [FIXED]
- Several remove funcions kept id refrences.
- Action serialization and debug.

### **1.0.0 - 2021-02-11**

### Added
- Dynamic node change by object picker.
- Embedded Build-In Assets

### Changed
- Removed ablity to remove Root Node.

### Fixed
- Rule removal no longer leaves 'ghost threads'.

##[0.9.0] - 2021-02-11

### Added
- Initalized embedded asset system.
- Xml class and function documentation.
- Ability to remove nodes by right clicking.
- Ability to remove a single Thread from a Port by righ clicking port.
- Finalized RuleWindow.
- Finalized ProfileLibrary.
- CHANGELOG.md initialization.

### Changed
- Action Thread loading now functions.