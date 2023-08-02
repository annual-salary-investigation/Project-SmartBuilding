#include <ArduinoJson.h> // ArduinoJson 라이브러리를 사용하기 위해 포함

DynamicJsonDocument doc(128); // JSON 문서 생성 (크기: 128 바이트)

void setup() {
  Serial.begin(9600); // 시리얼 통신 시작 (보레이트: 9600)
}

int i = 0;

void loop() {
  i = i + 1; // i를 1씩 증가시킴

  doc["AD1"] = i; // JSON 문서에 "AD1" 키로 i 값을 저장

  String jsonStr; // JSON 문자열을 저장할 변수 선언

  serializeJson(doc, jsonStr); // JSON 문서를 문자열로 변환하여 jsonStr에 저장

  Serial.println(jsonStr); // 시리얼 모니터에 JSON 문자열 출력

  delay(1000); // 1초 대기
}