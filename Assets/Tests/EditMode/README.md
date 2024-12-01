# Directory for Unit Tests

### Edit Mode Testing in Unity

- 주로 단순한 연산을 수행하는 함수의 테스트를 위해 사용
    - e.g., `AddLife()`, `SetScore()`, `GetCurrentPhase()`, etc. 

### About Edit Mode Tests
https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/edit-mode-vs-play-mode-tests.html#edit-mode-tests

### How to run edit mode tests?
1. Unity에서 `Window > General > TestRunner` 클릭해서 TestRunner 창 열기
2. TestRunner 창에서 Edit Mode 선택
    - `Run All`을 클릭하면 repository에 있는 모든 edit mode test 수행됨
    - Test Class를 더블 클릭하면 그 클래스 내에 구현된 모든 Test Function들이 펼쳐지고, Test Function을 더블 클릭하면 해당 Test Case만 수행됨
