# Roadmap

## High Priority
- Global Options UI
- Multiple References on one Reactive object cause state updates only for one reference

## Medium Priority
- Synchronize node interface value and the corresponding node option value
- Add tests
- Cleanup frontend code
- Connection status display in UI
- Automatic reconnect
- Don't allow multiple connections from one node interface to another
- Selection of multiple nodes (ideally with selection rectangle like on Windows desktop)
- Translate all nodes when dragging in empty part of node editor

## Low Priority
- Fix connections getting displayed wrong when hovering over node interface
- Improve general connection positioning
- Complete UI rework using dark controls
- Fix double conversion bug in number options
- Fix "Mix Color" node

## Bugs
- After the first output node, every subsequent output node is missing the output selection option
- Loading
- Binding -> Add another output node, somehow the full state as well as the state updates are put into it