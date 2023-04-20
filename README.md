# ProceduralGeneration
절차적맵생성 BSP

BSP(이진공간분할기법)는 주로 로그라이크게임에 사용되는 맵 생성 알고리즘 입니다.

1. 공간을 나눌 비율을 설정을 해줍니다 (6:4) 
2. depth가 높을수록 더욱 세밀하게 공간을 나누어줄 수 있습니다. 
3. 공간을 각각 left,rgiht Node에 저장해줍니다.
4. 저장한 node영역안에 방을 생성해줍니다.
5. 방을생성한 후 노드를 거슬러올라가 길을 연결해줍니다.