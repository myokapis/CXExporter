using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Exporter
{

    //public interface IDataMapper
    //{
    //    IEnumerable<IModelAccessor> GetModel(DbDataRecord dataRecord);
    //    void Initialize(DbDataReader dataReader);
    //}

    //public abstract class DataMapper //: IDataMapper
    //{
    //    protected bool hasCompositeData = false;
    //    protected Type modelType;
    //    protected string keyFieldName;

    //    //public DataMapper(Type modelType)
    //    //{
    //    //    this.modelType = modelType;
    //    //}

    //    //public DataMapper(Type modelType, string keyFieldName)
    //    //{
    //    //    hasCompositeData = true;
    //    //    this.modelType =  modelType;
    //    //    this.keyFieldName = keyFieldName;
    //    //}

    //    public abstract IEnumerable<IModelAccessor> GetModels(IDataReader dataReader);

    //}

    public class DataMapper
    {
        //public ModelDataMapper(Type modelType)
        //{
        //    this.modelType = modelType;
        //}

        //public ModelDataMapper(Type modelType, string keyFieldName)
        //{ 
        
        //}

        //public override IEnumerable<IModelAccessor> GetModels(IDataReader dataReader) =>
        //    hasCompositeData ? MapComposite() : MapSimple();

        //private IEnumerable<IModelAccessor> MapHorizontal()
        //{
        //    // TODO: implement
        //    yield return null;
        //}

        //private IEnumerable<IModelAccessor> MapVertical()
        //{
        //    // TODO: implement
        //    yield return null;
        //}

    }

    //public class ConfigDataMapper : DataMapper
    //{
    //    public ConfigDataMapper(Type modelType) : base(modelType) { }

    //    public ConfigDataMapper(Type modelType, string keyFieldName) : base(modelType, keyFieldName) { }

    //    public override IEnumerable<IModelAccessor> GetModels(IDataReader dataReader) =>
    //        hasCompositeData ? MapComposite() : MapSimple();

    //    private IEnumerable<IModelAccessor> MapComposite()
    //    {
    //        // TODO: implement
    //        yield return null;
    //    }

    //    private IEnumerable<IModelAccessor> MapSimple()
    //    {
    //        // TODO: implement
    //        yield return null;
    //    }

    //}

}
