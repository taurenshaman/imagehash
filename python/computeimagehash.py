import sys
from PIL import Image
import imagehash

def compute_whash(imagepath):
	imgfile = Image.open(imagepath)
	hash = imagehash.whash(imgfile)
	return hash


if __name__ == '__main__':
    imgfile = sys.argv[1]
    hash = compute_whash(imgfile)
    #hash = compute_whash('C:\Users\jerin\Desktop\imagehash\lenna.png')
    print hash
	#return hash
