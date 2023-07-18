import cv2
import numpy as np
import matplotlib.pyplot as plt


class Preprocess() :
    def __init__(self,jpg) :
        img_ori=cv2.imread(jpg)
        self.height, self.width, self.channel = img_ori.shape

        #hsv = cv2.cvtColor(img_ori, cv2.COLOR_BGR2HSV)
        #gray = hsv[:,:,2]
        gray=cv2.cvtColor(img_ori, cv2.COLOR_BGR2GRAY) #cv2.cvtColor() 이미지의 컬러 체계를 변경

        img_blurred=cv2.GaussianBlur(gray, ksize=(5,5), sigmaX=0) #노이즈 줄임

        self.img_thresh=cv2.adaptiveThreshold( 
            img_blurred,
            maxValue=255.0,
            adaptiveMethod=cv2.ADAPTIVE_THRESH_GAUSSIAN_C,
            thresholdType=cv2.THRESH_BINARY_INV,
            blockSize=19,
            C=9
        )

        contours,_ = cv2.findContours( #이미지에서 윤곽선을 찾는다
            self.img_thresh, 
            mode=cv2.RETR_LIST, 
            method=cv2.CHAIN_APPROX_SIMPLE
        )

        temp_result = np.zeros((self.height, self.width, self.channel), dtype=np.uint8)

        cv2.drawContours(temp_result, contours=contours, contourIdx=-1 , color=(255, 255, 255))
        #윤곽선을 그린다 -1주면 전체 윤곽선을 그리겠다는 뜻

        #번호판의 위치를 찾기 쉽게 하기 위해서
        temp_result = np.zeros((self.height,self.width,self.channel),dtype=np.uint8)

        contours_dict=[]

        for contour in contours:
            x,y,w,h = cv2.boundingRect(contour) #cv2.boundingRect() 윤곽선을 감싸는 사각형을 구한다.
            #그후 윤곽선을 감싸는 사각형의 x,y좌표와 너비와 높이 저장
            # cv2.rectangle(temp_result, pt1=(x,y), pt2=(x+w,y+h),color=(255,255,255),thickness=2)
            #cv2.rectangle() 이미지에 사각형을 그린다.
            #insert to dict
            contours_dict.append({
                'contour':contour,
                'x':x,
                'y':y,
                'w':w,
                'h':h,
                'cx':x+(w/2), #사각형의 중심좌표
                'cy':y+(h/2)
            })

        self.MIN_AREA = 80  #번호판 숫자 번호판 바운딩 렉트의 최소 넓이
        self.MIN_WIDTH, self.MIN_HEIGHT = 2, 8 #바운딩 렉트의 최소 너비와 높이
        self.MIN_RATIO, self.MAX_RATIO = 0.25, 1.0 #번호판 바운딩 렉트의 가로대비 세로 비율 최소,최대

        self.possible_contours = [] #가능한 애들 여기에 저장

        cnt = 0
        for d in contours_dict:
            area = d['w'] * d['h'] #넓이
            ratio = d['w'] / d['h'] #가로 대비 세로 비율
            
            if area > self.MIN_AREA \
            and d['w'] > self.MIN_WIDTH and d['h'] > self.MIN_HEIGHT \
            and self.MIN_RATIO < ratio < self.MAX_RATIO:
                d['idx'] = cnt #각 윤곽선의 index값 저장
                cnt += 1
                self.possible_contours.append(d) #위 조건에 맞는 사각형들을 possible_contours에 저장
                
        # visualize possible contours
        temp_result = np.zeros((self.height, self.width, self.channel), dtype=np.uint8)

        for d in self.possible_contours:
        #     cv2.drawContours(temp_result, d['contour'], -1, (255, 255, 255))
            cv2.rectangle(temp_result, pt1=(d['x'], d['y']), pt2=(d['x']+d['w'], d['y']+d['h']), color=(255, 255, 255), thickness=2)

        self.MAX_DIAG_MULTIPLYER = 5 # 5      #사각형과 사각형 길이 제한 #첫번째 사각형의 대각선의 다섯배 안에 다음 사각형이 있어야함
        self.MAX_ANGLE_DIFF = 12.0 # 12.0 #첫번째 사각형과 두번째 사각형 중심을 이어 직각삼각형을 만들고 첫번째 사각형쪽 각도의 최대값
        self.MAX_AREA_DIFF = 0.5 # 0.5 #사각형과 사각형 면적차이 제한
        self.MAX_WIDTH_DIFF = 0.8 #너비차이
        self.MAX_HEIGHT_DIFF = 0.2 #높이차이
        self.MIN_N_MATCHED = 3 # 3 #위의 값을 만족하는 값들이 3개 이상이어야함

    def find_chars(self,contour_list): 
        matched_result_idx = [] #최종 인덱스값들을 저장할것
        
        for d1 in contour_list: 
            matched_contours_idx = []
            for d2 in contour_list:
                if d1['idx'] == d2['idx']: #idx가 같으면 같은 사각형이므로 패스
                    continue

                dx = abs(d1['cx'] - d2['cx'])
                dy = abs(d1['cy'] - d2['cy'])

                diagonal_length1 = np.sqrt(d1['w'] ** 2 + d1['h'] ** 2)

                distance = np.linalg.norm(np.array([d1['cx'], d1['cy']]) - np.array([d2['cx'], d2['cy']]))
                if dx == 0:
                    angle_diff = 90
                else:
                    angle_diff = np.degrees(np.arctan(dy / dx))
                area_diff = abs(d1['w'] * d1['h'] - d2['w'] * d2['h']) / (d1['w'] * d1['h'])
                width_diff = abs(d1['w'] - d2['w']) / d1['w']
                height_diff = abs(d1['h'] - d2['h']) / d1['h']

                if distance < diagonal_length1 * self.MAX_DIAG_MULTIPLYER \
                and angle_diff < self.MAX_ANGLE_DIFF and area_diff < self.MAX_AREA_DIFF \
                and width_diff < self.MAX_WIDTH_DIFF and height_diff < self.MAX_HEIGHT_DIFF:
                    matched_contours_idx.append(d2['idx'])

            # append this contour
            matched_contours_idx.append(d1['idx'])

            if len(matched_contours_idx) < self.MIN_N_MATCHED:
                continue

            matched_result_idx.append(matched_contours_idx)

            unmatched_contour_idx = []
            for d4 in contour_list:
                if d4['idx'] not in matched_contours_idx:
                    unmatched_contour_idx.append(d4['idx'])

            unmatched_contour = np.take(self.possible_contours, unmatched_contour_idx)
            
            # recursive
            recursive_contour_list = self.find_chars(unmatched_contour)
            
            for idx in recursive_contour_list:
                matched_result_idx.append(idx)

            break

        return matched_result_idx
    
    def run(self) :
        result_idx = self.find_chars(self.possible_contours)

        matched_result = []
        for idx_list in result_idx:
            matched_result.append(np.take(self.possible_contours, idx_list))

        # visualize possible contours
        temp_result = np.zeros((self.height, self.width, self.channel), dtype=np.uint8)

        for r in matched_result:
            for d in r:
        #         cv2.drawContours(temp_result, d['contour'], -1, (255, 255, 255))
                cv2.rectangle(temp_result, pt1=(d['x'], d['y']), pt2=(d['x']+d['w'], d['y']+d['h']), color=(255, 255, 255), thickness=2)

        
        PLATE_WIDTH_PADDING = 1.3 # 1.3
        PLATE_HEIGHT_PADDING = 1.5 # 1.5
        MIN_PLATE_RATIO = 3
        MAX_PLATE_RATIO = 10

        plate_imgs = []
        plate_infos = []

        for i, matched_chars in enumerate(matched_result):
            sorted_chars = sorted(matched_chars, key=lambda x: x['cx'])

            plate_cx = (sorted_chars[0]['cx'] + sorted_chars[-1]['cx']) / 2
            plate_cy = (sorted_chars[0]['cy'] + sorted_chars[-1]['cy']) / 2
            
            plate_width = (sorted_chars[-1]['x'] + sorted_chars[-1]['w'] - sorted_chars[0]['x']) * PLATE_WIDTH_PADDING
            
            sum_height = 0
            for d in sorted_chars:
                sum_height += d['h']

            plate_height = int(sum_height / len(sorted_chars) * PLATE_HEIGHT_PADDING)
            
            triangle_height = sorted_chars[-1]['cy'] - sorted_chars[0]['cy']
            triangle_hypotenus = np.linalg.norm(
                np.array([sorted_chars[0]['cx'], sorted_chars[0]['cy']]) - 
                np.array([sorted_chars[-1]['cx'], sorted_chars[-1]['cy']])
            )
            
            angle = np.degrees(np.arcsin(triangle_height / triangle_hypotenus))
            
            rotation_matrix = cv2.getRotationMatrix2D(center=(plate_cx, plate_cy), angle=angle, scale=1.0)
            
            img_rotated = cv2.warpAffine(self.img_thresh, M=rotation_matrix, dsize=(self.width, self.height))
            
            img_cropped = cv2.getRectSubPix(
                img_rotated, 
                patchSize=(int(plate_width), int(plate_height)), 
                center=(int(plate_cx), int(plate_cy))
            )
            
            if img_cropped.shape[1] / img_cropped.shape[0] < MIN_PLATE_RATIO or img_cropped.shape[1] / img_cropped.shape[0] < MIN_PLATE_RATIO > MAX_PLATE_RATIO:
                continue
            
            plate_imgs.append(img_cropped)
            plate_infos.append({
                'x': int(plate_cx - plate_width / 2),
                'y': int(plate_cy - plate_height / 2),
                'w': int(plate_width),
                'h': int(plate_height)
            })

            for i, plate_img in enumerate(plate_imgs):
                plate_img = cv2.resize(plate_img, dsize=(0, 0), fx=1.6, fy=1.6)
                _, plate_img = cv2.threshold(plate_img, thresh=0.0, maxval=255.0, type=cv2.THRESH_BINARY | cv2.THRESH_OTSU)
                
                # find contours again (same as above)
                contours, _ = cv2.findContours(plate_img, mode=cv2.RETR_LIST, method=cv2.CHAIN_APPROX_SIMPLE)
                
                plate_min_x, plate_min_y = plate_img.shape[1], plate_img.shape[0]
                plate_max_x, plate_max_y = 0, 0

                for contour in contours:
                    x, y, w, h = cv2.boundingRect(contour)
                    
                    area = w * h
                    ratio = w / h

                    if area > self.MIN_AREA \
                    and w > self.MIN_WIDTH and h > self.MIN_HEIGHT \
                    and self.MIN_RATIO < ratio < self.MAX_RATIO:
                        if x < plate_min_x:
                            plate_min_x = x
                        if y < plate_min_y:
                            plate_min_y = y
                        if x + w > plate_max_x:
                            plate_max_x = x + w
                        if y + h > plate_max_y:
                            plate_max_y = y + h
                            
                self.img_result = plate_img[plate_min_y:plate_max_y, plate_min_x:plate_max_x]
                
                self.img_result = cv2.GaussianBlur(self.img_result, ksize=(3, 3), sigmaX=0)
                _, self.img_result = cv2.threshold(self.img_result, thresh=0.0, maxval=255.0, type=cv2.THRESH_BINARY | cv2.THRESH_OTSU)
                self.img_result = cv2.copyMakeBorder(self.img_result, top=10, bottom=10, left=10, right=10, borderType=cv2.BORDER_CONSTANT, value=(0,0,0))
    
