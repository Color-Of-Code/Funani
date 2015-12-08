from django.db import models

# Create your models here.
class Medium(models.Model):
    DEVICE_TYPES = (
        ('CDR', 'CD-ROM'),
        ('DVD', 'DVD'),
        ('HDD', 'Hard Disk Drive'),
    )
    label = models.CharField(max_length=64)
    device_type = models.CharField(max_length=3, choices=DEVICE_TYPES)
    serial_number = models.CharField(max_length=64)
    base_path = models.CharField(max_length=200)
    question_text = models.CharField(max_length=200)
    created_date = models.DateTimeField()
    updated_date = models.DateTimeField()



