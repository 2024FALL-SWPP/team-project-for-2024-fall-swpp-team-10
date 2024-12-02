# Directory for Integrated Tests

### Play Mode Testing in Unity

- 게임 플레이를 간단히 재현해보는 테스트를 위해 사용
    - e.g., `LoadScene()`
    - 더 많은 예시는 생각나는대로 추가하겠음

### About Play Mode Tests
https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/edit-mode-vs-play-mode-tests.html#play-mode-tests

### How to run play mode tests?
> 반드시 비어있는 SampleScene에서 테스트를 수행하세요.

1. Unity에서 `Window > General > TestRunner` 클릭해서 TestRunner 창 열기
2. TestRunner 창에서 Play Mode 선택
    - `Run All`을 클릭하면 repository에 있는 모든 play mode test 수행됨
    - Test Class를 더블 클릭하면 그 클래스 내에 구현된 모든 Test Function들이 펼쳐지고, Test Function을 더블 클릭하면 해당 Test Case만 수행됨
