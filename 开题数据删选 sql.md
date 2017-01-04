```sql
SELECT CONVERT(varchar(13),AcquisitionTime,120) as time
      ,avg([InletPressure]) as 进水压力
      ,avg([OutletPressure]) as 出水压力
      ,avg([Frequency]) as 频率
      ,avg([PowerV]) as 功率

  FROM [AKGS].[dbo].[TB_AS_21]
  GROUP BY CONVERT(varchar(13),AcquisitionTime,120)
  ORDER BY CONVERT(varchar(13),AcquisitionTime,120)
```